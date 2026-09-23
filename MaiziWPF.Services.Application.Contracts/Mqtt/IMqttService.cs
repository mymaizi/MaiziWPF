using System;
using System.Threading.Tasks;
using MaiziWPF.Services.Domain;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface IMqttService : ISingletonDependency
    {
        event Action<List<SysMessage>> OnNewMessages;
        event Action<string> OnKicked;
        event Action<bool> OnConnectionChanged;

        Task StartAsync(long userId, string serverIp, int serverPort = 1883);
        Task StopAsync();
        Task PublishGlobalAsync(long messageId);
        Task PublishToUserAsync(long userId, long messageId);
        Task KickUserAsync(long userId, string reason);
        Task DisableUserAsync(long userId, string reason);
        bool IsConnected { get; }
    }
}