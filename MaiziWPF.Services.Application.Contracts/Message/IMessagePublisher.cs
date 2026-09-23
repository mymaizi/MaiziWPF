using MaiziWPF.Services.Domain;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface IMessagePublisher : ITransientDependency
    {
        void PublishNotice(SysNotice notice);

        void PublishSystemMessage(string title, string message, string path = null);

        void PublishWorkflowMessage(string title, string message, long userId, string path = null);
    }
}