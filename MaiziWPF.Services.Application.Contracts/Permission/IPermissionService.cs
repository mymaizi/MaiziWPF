using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface IPermissionService : ITransientDependency
    {
        void LoadUserPermissions(long userId);

        void Clear();
    }
}