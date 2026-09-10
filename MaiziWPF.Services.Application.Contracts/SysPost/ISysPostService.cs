using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface ISysPostService : ITransientDependency
    {
        List<SysPost> SelectPostList(QueryPostInput post);

        SysPost SelectPostById(long postId);

        int InsertPost(SysPost post);

        int UpdatePost(SysPost post);

        int DeletePostById(long postId);

        bool CheckPostNameUnique(SysPost post);

        bool CheckPostCodeUnique(SysPost post);

        bool CheckPostExistUser(long postId);
    }
}