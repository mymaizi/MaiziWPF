using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace MaiziWPF.Common.Oss
{
    public class S3ClientService : IS3ClientService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly OssOptions _options;
        private readonly ILogger<S3ClientService> _logger;

        public S3ClientService(IOptions<OssOptions> options, ILogger<S3ClientService> logger)
        {
            _options = options.Value;
            _logger = logger;

            var config = new AmazonS3Config
            {
                ServiceURL = _options.Endpoint,
                ForcePathStyle = _options.ForcePathStyle,
                Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds),
                MaxErrorRetry = _options.MaxErrorRetry,
                UseHttp = !_options.UseHttps
            };

            var credentials = new BasicAWSCredentials(_options.AccessKey, _options.SecretKey);
            _s3Client = new AmazonS3Client(credentials, config);
        }

        public async Task<OssUploadResult> UploadAsync(string filePath, string? customKey = null, IProgress<int>? progress = null)
        {
            if (!File.Exists(filePath))
                return ErrorResult($"文件不存在: {filePath}");

            var originalName = Path.GetFileName(filePath);
            var key = customKey ?? BuildKey(originalName);
            var contentType = GetContentType(filePath);

            try
            {
                var fileInfo = new FileInfo(filePath);
                var request = new PutObjectRequest
                {
                    BucketName = _options.BucketName,
                    Key = key,
                    FilePath = filePath,
                    ContentType = contentType
                };

                if (progress != null && fileInfo.Length > 0)
                {
                    var totalBytes = fileInfo.Length;
                    request.FilePath = null;
                    var fileStream = new ProgressStream(File.OpenRead(filePath), totalBytes, progress);
                    request.InputStream = fileStream;
                }

                var response = await _s3Client.PutObjectAsync(request);

                _logger.LogInformation("文件上传成功: {Key}, ETag: {ETag}", key, response.ETag);

                return new OssUploadResult
                {
                    Success = true,
                    Key = key,
                    OriginalName = originalName,
                    FileName = key,
                    FileSuffix = Path.GetExtension(originalName).TrimStart('.'),
                    Size = new FileInfo(filePath).Length,
                    ContentType = contentType
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "文件上传失败: {FilePath}", filePath);
                return ErrorResult($"上传失败: {ex.Message}");
            }
        }

        public async Task<OssUploadResult> UploadAsync(Stream stream, string originalName, string? customKey = null, IProgress<int>? progress = null)
        {
            var key = customKey ?? BuildKey(originalName);
            var contentType = GetContentType(originalName);

            try
            {
                var request = new PutObjectRequest
                {
                    BucketName = _options.BucketName,
                    Key = key,
                    InputStream = progress != null && stream.CanSeek && stream.Length > 0
                        ? new ProgressStream(stream, stream.Length, progress)
                        : stream,
                    ContentType = contentType
                };

                var response = await _s3Client.PutObjectAsync(request);

                _logger.LogInformation("流式上传成功: {Key}, ETag: {ETag}", key, response.ETag);

                return new OssUploadResult
                {
                    Success = true,
                    Key = key,
                    OriginalName = originalName,
                    FileName = key,
                    FileSuffix = Path.GetExtension(originalName).TrimStart('.'),
                    ContentType = contentType
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "流式上传失败: {OriginalName}", originalName);
                return ErrorResult($"上传失败: {ex.Message}");
            }
        }

        public async Task<Stream> DownloadAsync(string key)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = _options.BucketName,
                    Key = key
                };

                var response = await _s3Client.GetObjectAsync(request);
                return response.ResponseStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "文件下载失败: {Key}", key);
                throw;
            }
        }

        public async Task DeleteAsync(string key)
        {
            try
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = _options.BucketName,
                    Key = key
                };

                await _s3Client.DeleteObjectAsync(request);
                _logger.LogInformation("文件删除成功: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "文件删除失败: {Key}", key);
                throw;
            }
        }

        public async Task<bool> ObjectExistsAsync(string key)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _options.BucketName,
                    Key = key
                };

                await _s3Client.GetObjectMetadataAsync(request);
                return true;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        public string GetAccessUrl(string key)
        {
            var endpoint = _options.Endpoint.TrimEnd('/');
            return $"{endpoint}/{_options.BucketName}/{key}";
        }

        public async Task<bool> EnsureBucketExistsAsync()
        {
            try
            {
                var listResponse = await _s3Client.ListBucketsAsync();
                var exists = listResponse.Buckets.Any(b => b.BucketName == _options.BucketName);
                if (!exists)
                {
                    var createRequest = new PutBucketRequest
                    {
                        BucketName = _options.BucketName
                    };
                    await _s3Client.PutBucketAsync(createRequest);
                    _logger.LogInformation("桶创建成功: {BucketName}", _options.BucketName);
                }
                return true;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                _logger.LogWarning("桶已存在或无权限创建: {BucketName}", _options.BucketName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "桶检查/创建失败: {BucketName}", _options.BucketName);
                return false;
            }
        }

        public async Task<List<S3Object>> ListObjectsAsync(string? prefix = null)
        {
            try
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = _options.BucketName,
                    Prefix = prefix ?? _options.Prefix
                };

                var response = await _s3Client.ListObjectsV2Async(request);
                return response.S3Objects.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "列举对象失败: {Prefix}", prefix);
                throw;
            }
        }

        private string BuildKey(string originalName)
        {
            var now = DateTime.Now;
            var datePath = now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            var uuid = Guid.NewGuid().ToString("N")[..8];
            var suffix = Path.GetExtension(originalName);
            return $"{_options.Prefix}/{datePath}/{uuid}{suffix}";
        }

        private static string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".txt" => "text/plain",
                ".csv" => "text/csv",
                _ => "application/octet-stream"
            };
        }

        private static OssUploadResult ErrorResult(string message)
        {
            return new OssUploadResult { Success = false, ErrorMessage = message };
        }
    }

    internal class ProgressStream : Stream
    {
        private readonly Stream _inner;
        private readonly long _totalBytes;
        private readonly IProgress<int> _progress;
        private long _bytesRead;
        private int _lastReported;

        public ProgressStream(Stream inner, long totalBytes, IProgress<int> progress)
        {
            _inner = inner;
            _totalBytes = totalBytes;
            _progress = progress;
        }

        public override bool CanRead => _inner.CanRead;
        public override bool CanSeek => _inner.CanSeek;
        public override bool CanWrite => false;
        public override long Length => _inner.Length;
        public override long Position { get => _inner.Position; set => _inner.Position = value; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = _inner.Read(buffer, offset, count);
            ReportProgress(read);
            return read;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var read = await _inner.ReadAsync(buffer, offset, count, cancellationToken);
            ReportProgress(read);
            return read;
        }

        private void ReportProgress(int bytesRead)
        {
            if (bytesRead <= 0) return;
            _bytesRead += bytesRead;
            var percent = (int)(_bytesRead * 100 / _totalBytes);
            if (percent != _lastReported)
            {
                _lastReported = percent;
                _progress.Report(percent);
            }
        }

        public override void Flush() => _inner.Flush();
        public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);
        public override void SetLength(long value) => _inner.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing) _inner.Dispose();
            base.Dispose(disposing);
        }
    }
}