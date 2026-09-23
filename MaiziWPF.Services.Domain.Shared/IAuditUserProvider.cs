namespace MaiziWPF.Services.Domain.Shared
{
    public interface IAuditUserProvider
    {
        long UserId { get; }

        long DeptId { get; }

        bool IsAuthenticated { get; }
    }
}