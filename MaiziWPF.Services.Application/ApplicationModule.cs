﻿using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Volo.Abp.Modularity;

namespace MaiziWPF.Services.Application
{
    [DependsOn(
         typeof(DomainModule),
         typeof(ApplicationContractsModule)
    )]
    public class ApplicationModule : AbpModule
    {
   
    }
}