namespace MaiziWPF.Common.Oss
{
    public class OssOptions
    {
        public string Endpoint { get; set; } = "http://127.0.0.1:9000";
        public string AccessKey { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public string BucketName { get; set; } = "maizi-wpf";
        public bool ForcePathStyle { get; set; } = true;
        public bool UseHttps { get; set; } = false;
        public string Prefix { get; set; } = "upload";
        public int TimeoutSeconds { get; set; } = 300;
        public int MaxErrorRetry { get; set; } = 1;
    }
}