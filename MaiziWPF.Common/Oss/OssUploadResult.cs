namespace MaiziWPF.Common.Oss
{
    public class OssUploadResult
    {
        public bool Success { get; set; }
        public string Key { get; set; } = "";
        public string OriginalName { get; set; } = "";
        public string FileName { get; set; } = "";
        public string FileSuffix { get; set; } = "";
        public long Size { get; set; }
        public string ContentType { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
    }
}