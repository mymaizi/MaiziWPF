using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaiziWPF.Services.Application
{
    public class SysConfigService : ISysConfigService
    {
        private readonly ISysConfigRepository _repository;

        public SysConfigService(ISysConfigRepository repository)
        {
            _repository = repository;
        }

        public List<SysConfig> SelectConfigList(QueryConfigInput input)
        {
            return _repository.SelectConfigList(input);
        }

        public SysConfig SelectConfigById(long configId)
        {
            return _repository.SelectConfigById(configId);
        }

        public int InsertConfig(SysConfig config)
        {
            return _repository.InsertConfig(config);
        }

        public int UpdateConfig(SysConfig config)
        {
            return _repository.UpdateConfig(config);
        }

        public int DeleteConfigById(long configId)
        {
            return _repository.DeleteConfigById(configId);
        }

        public bool CheckConfigKeyUnique(SysConfig config)
        {
            return _repository.CheckConfigKeyUnique(config);
        }
    }
}