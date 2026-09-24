using Amazon.S3.Model;

namespace MaiziWPF.Common.Oss
{
    public interface IS3ClientService
    {
        Task<OssUploadResult> UploadAsync(string filePath, string? customKey = null, IProgress<int>? progress = null);

        Task<OssUploadResult> UploadAsync(Stream stream, string originalName, string? customKey = null, IProgress<int>? progress = null);

        Task<Stream> DownloadAsync(string key);

        Task DeleteAsync(string key);

        Task<bool> ObjectExistsAsync(string key);

        string GetAccessUrl(string key);

        Task<bool> EnsureBucketExistsAsync();

        Task<List<S3Object>> ListObjectsAsync(string? prefix = null);
    }
}