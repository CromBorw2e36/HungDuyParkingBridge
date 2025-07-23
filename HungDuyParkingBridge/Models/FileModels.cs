namespace HungDuyParkingBridge.Models
{
    public class FileMetadata
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string MimeType { get; set; } = string.Empty;
        public bool IsCompressed { get; set; }
        public string? ParentId { get; set; }
        public bool IsFolder { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}