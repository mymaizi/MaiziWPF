using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application
{
    public class MqttService : IMqttService, ISingletonDependency
    {
        private IMqttClient _client;
        private readonly ISysMessageService _messageService;
        private readonly ILocalDbService _localDb;
        private readonly ILogger<MqttService> _logger;
        private long _userId;
        private bool _isConnected;

        public event Action<List<SysMessage>> OnNewMessages;
        public event Action<string> OnKicked;
        public event Action<bool> OnConnectionChanged;

        public bool IsConnected => _isConnected;

        public MqttService(ISysMessageService messageService, ILocalDbService localDb, ILogger<MqttService> logger)
        {
            _messageService = messageService;
            _localDb = localDb;
            _logger = logger;
        }

        public async Task StartAsync(long userId, string serverIp, int serverPort = 1883)
        {
            _userId = userId;
            _localDb.Init();

            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId($"user_{userId}")
                .WithTcpServer(serverIp, serverPort)
                .WithCleanSession(false)
                .WithWillTopic($"offline/{userId}")
                .WithWillPayload("0")
                .WithWillRetain(true)
                .Build();

            _client.ConnectedAsync += async e =>
            {
                _isConnected = true;
                OnConnectionChanged?.Invoke(true);

                await _client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("message/all").WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).Build());
                await _client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic($"message/user/{userId}").WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).Build());
                await _client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic($"user/{userId}/control").WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).Build());

                await PullOfflineMessagesAsync();
            };

            _client.DisconnectedAsync += async e =>
            {
                _isConnected = false;
                OnConnectionChanged?.Invoke(false);
            };

            _client.ApplicationMessageReceivedAsync += async e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                _logger.LogInformation("[MQTT-Receive] 收到消息: Topic={Topic}, Payload={Payload}", topic, payload);

                if (topic == "message/all" || topic.StartsWith("message/user/"))
                {
                    var signal = JsonConvert.DeserializeObject<MsgSignal>(payload);
                    _logger.LogInformation("[MQTT-Receive] 解析消息: MessageId={MessageId}", signal?.MessageId);
                    await HandleNewMessageAsync(signal);
                }
                else if (topic == $"user/{userId}/control")
                {
                    var cmd = JsonConvert.DeserializeObject<ControlCommand>(payload);
                    HandleControlCommand(cmd);
                }
            };

            await _client.ConnectAsync(options);
        }

        private async Task PullOfflineMessagesAsync()
        {
            var lastId = _localDb.GetLastSyncId(_userId);
            var messages = _messageService.SelectNewMessages(lastId, _userId);

            if (messages != null && messages.Count > 0)
            {
                foreach (var msg in messages)
                    _localDb.SaveMessage(_userId, msg.MessageId);

                _localDb.SetLastSyncId(_userId, messages[messages.Count - 1].MessageId);
                OnNewMessages?.Invoke(messages);
            }
        }

        private async Task HandleNewMessageAsync(MsgSignal signal)
        {
            _localDb.SaveMessage(_userId, signal.MessageId);
            _localDb.SetLastSyncId(_userId, signal.MessageId);

            var msg = _messageService.SelectMessageById(signal.MessageId);
            if (msg != null)
                OnNewMessages?.Invoke(new List<SysMessage> { msg });
        }

        private void HandleControlCommand(ControlCommand cmd)
        {
            switch (cmd.Action)
            {
                case "kick":
                case "disable":
                    OnKicked?.Invoke(cmd.Reason ?? "您已被管理员强制下线");
                    break;
            }
        }

        public async Task PublishGlobalAsync(long messageId)
        {
            _logger.LogInformation("[MQTT-Publish] 开始发布全局消息: MessageId={MessageId}, IsConnected={IsConnected}", messageId, _client?.IsConnected);
            var signal = new MsgSignal { MessageId = messageId };
            await _client.PublishAsync(new MqttApplicationMessageBuilder()
                .WithTopic("message/all")
                .WithPayload(JsonConvert.SerializeObject(signal))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build());
            _logger.LogInformation("[MQTT-Publish] 发布全局消息成功: MessageId={MessageId}", messageId);
        }

        public async Task PublishToUserAsync(long targetUserId, long messageId)
        {
            var signal = new MsgSignal { MessageId = messageId };
            await _client.PublishAsync(new MqttApplicationMessageBuilder()
                .WithTopic($"message/user/{targetUserId}")
                .WithPayload(JsonConvert.SerializeObject(signal))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build());
        }

        public async Task KickUserAsync(long targetUserId, string reason)
        {
            await _client.PublishAsync(new MqttApplicationMessageBuilder()
                .WithTopic($"user/{targetUserId}/control")
                .WithPayload(JsonConvert.SerializeObject(new ControlCommand
                {
                    Action = "kick",
                    Reason = reason
                }))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build());
        }

        public async Task DisableUserAsync(long targetUserId, string reason)
        {
            await _client.PublishAsync(new MqttApplicationMessageBuilder()
                .WithTopic($"user/{targetUserId}/control")
                .WithPayload(JsonConvert.SerializeObject(new ControlCommand
                {
                    Action = "disable",
                    Reason = reason
                }))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build());
        }

        public async Task StopAsync()
        {
            if (_client != null && _client.IsConnected)
            {
                try
                {
                    await _client.PublishAsync(new MqttApplicationMessageBuilder()
                        .WithTopic($"offline/{_userId}")
                        .WithPayload("0")
                        .WithRetainFlag(true)
                        .Build());
                }
                catch { }

                await _client.DisconnectAsync();
            }
        }
    }

    internal class MsgSignal
    {
        public long MessageId { get; set; }
    }

    internal class ControlCommand
    {
        public string Action { get; set; }
        public string Reason { get; set; }
    }
}