using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MaiziWPF.Services.Application
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly ISysMessageService _messageService;
        private readonly IMqttService _mqttService;
        private readonly ILogger<MessagePublisher> _logger;

        public MessagePublisher(ISysMessageService messageService, IMqttService mqttService, ILogger<MessagePublisher> logger)
        {
            _messageService = messageService;
            _mqttService = mqttService;
            _logger = logger;
        }

        public void PublishNotice(SysNotice notice)
        {
            if (notice == null || notice.NoticeId <= 0) return;
            if (notice.NoticeType != "1") return; // 只有通知才创建消息

            var msg = new SysMessage
            {
                Category = "notice",
                Type = "notice",
                Source = "notice",
                Title = notice.NoticeTitle,
                Message = Truncate(notice.NoticeContent, 100),
                Content = notice.NoticeContent,
                SendUserIds = "0",
                Path = $"notice/detail/{notice.NoticeId}",
                CreateBy = notice.CreateBy
            };
            msg.MessageId = _messageService.InsertMessage(msg);

            // MQTT推送（异步不阻塞）
            _ = Task.Run(async () =>
            {
                try
                {
                    _logger.LogInformation("[MQTT] 开始推送: MessageId={MessageId}, Title={Title}", msg.MessageId, msg.Title);
                    await _mqttService.PublishGlobalAsync(msg.MessageId);
                    _logger.LogInformation("[MQTT] 推送成功: MessageId={MessageId}", msg.MessageId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[MQTT] 推送失败: MessageId={MessageId}", msg.MessageId);
                }
            });
        }

        public void PublishSystemMessage(string title, string message, string path = null)
        {
            var msg = new SysMessage
            {
                Category = "system",
                Type = "system",
                Source = "backend",
                Title = title,
                Message = message,
                Content = message,
                SendUserIds = "0",
                Path = path
            };
            msg.MessageId = _messageService.InsertMessage(msg);

            // MQTT推送（异步不阻塞）
            _ = Task.Run(async () =>
            {
                try
                {
                    _logger.LogInformation("[MQTT] 开始推送: MessageId={MessageId}, Title={Title}", msg.MessageId, msg.Title);
                    await _mqttService.PublishGlobalAsync(msg.MessageId);
                    _logger.LogInformation("[MQTT] 推送成功: MessageId={MessageId}", msg.MessageId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[MQTT] 推送失败: MessageId={MessageId}", msg.MessageId);
                }
            });
        }

        public void PublishWorkflowMessage(string title, string message, long userId, string path = null)
        {
            var msg = new SysMessage
            {
                Category = "workflow",
                Type = "workflow",
                Source = "workflow",
                Title = title,
                Message = message,
                Content = message,
                SendUserIds = userId.ToString(),
                Path = path
            };
            msg.MessageId = _messageService.InsertMessage(msg);

            // MQTT推送给指定用户（异步不阻塞）
            _ = Task.Run(async () =>
            {
                try
                {
                    _logger.LogInformation("[MQTT] 开始推送给用户: UserId={UserId}, MessageId={MessageId}", userId, msg.MessageId);
                    await _mqttService.PublishToUserAsync(userId, msg.MessageId);
                    _logger.LogInformation("[MQTT] 推送成功给用户: UserId={UserId}, MessageId={MessageId}", userId, msg.MessageId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[MQTT] 推送失败给用户: UserId={UserId}, MessageId={MessageId}", userId, msg.MessageId);
                }
            });
        }

        private static string Truncate(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return text.Length > maxLength ? text.Substring(0, maxLength) + "..." : text;
        }
    }
}