using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface ISysMessageService : ITransientDependency
    {
        List<SysMessage> SelectMessageList(QueryMessageInput input);

        SysMessage SelectMessageById(long messageId);

        long InsertMessage(SysMessage msg);

        List<SysMessage> SelectNewMessages(long lastId, long userId);

        List<SysMessage> SelectMessagesByIds(List<long> msgIds);
    }
}