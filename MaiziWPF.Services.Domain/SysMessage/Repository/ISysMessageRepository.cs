using FreeSql;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Domain
{
    public interface ISysMessageRepository : IBaseRepository<SysMessage, int>, ITransientDependency
    {
        List<SysMessage> SelectMessageList(QueryMessageInput input);

        SysMessage SelectMessageById(long messageId);

        long InsertMessage(SysMessage msg);

        List<SysMessage> SelectNewMessages(long lastId, long userId);

        List<SysMessage> SelectMessagesByIds(List<long> msgIds);
    }
}