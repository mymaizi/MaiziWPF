using FreeSql;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Domain
{
    public interface ISysPostRepository : IBaseRepository<SysPost, int>, ITransientDependency
    {
        List<SysPost> SelectPostList(QueryPostInput input);

        SysPost SelectPostById(long postId);

        int InsertPost(SysPost post);

        int UpdatePost(SysPost post);

        int DeletePostById(long postId);

        bool CheckPostNameUnique(SysPost post);

        bool CheckPostCodeUnique(SysPost post);

        bool CheckPostExistUser(long postId);
    }
}