namespace MaiziWPF.Services.Domain.Shared
{
    public interface IDataPermission
    {
        long CreateDept { get; }

        long CreateBy { get; }
    }
}