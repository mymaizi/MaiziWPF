using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application
{
    public interface ISysConfigService : ITransientDependency
    {
        List<SysConfig> SelectConfigList(QueryConfigInput input);

        SysConfig SelectConfigById(long configId);

        int InsertConfig(SysConfig config);

        int UpdateConfig(SysConfig config);

        int DeleteConfigById(long configId);

        bool CheckConfigKeyUnique(SysConfig config);
    }
}