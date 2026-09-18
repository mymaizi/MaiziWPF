namespace MaiziWPF.Services.Domain.Shared
{
    public interface IPagingInfo
    {
        int PageNumber { get; set; }
        int PageSize { get; set; }
        long Count { get; set; }
    }
}