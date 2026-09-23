using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;

namespace MaiziWPF.Services.Application
{
    public class SysMessageService : ISysMessageService
    {
        private readonly ISysMessageRepository _repository;

        public SysMessageService(ISysMessageRepository repository)
        {
            _repository = repository;
        }

        public List<SysMessage> SelectMessageList(QueryMessageInput input)
        {
            return _repository.SelectMessageList(input);
        }

        public SysMessage SelectMessageById(long messageId)
        {
            return _repository.SelectMessageById(messageId);
        }

        public long InsertMessage(SysMessage msg)
        {
            return _repository.InsertMessage(msg);
        }

        public List<SysMessage> SelectNewMessages(long lastId, long userId)
        {
            return _repository.SelectNewMessages(lastId, userId);
        }

        public List<SysMessage> SelectMessagesByIds(List<long> msgIds)
        {
            return _repository.SelectMessagesByIds(msgIds);
        }
    }
}