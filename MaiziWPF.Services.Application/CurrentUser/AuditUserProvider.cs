using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application
{
    public class AuditUserProvider : IAuditUserProvider, ISingletonDependency
    {
        private readonly ICurrentUserService _currentUserService;

        public AuditUserProvider(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public long UserId => _currentUserService.UserId;
        public long DeptId => _currentUserService.DeptId;
        public bool IsAuthenticated => _currentUserService.IsAuthenticated;
    }
}