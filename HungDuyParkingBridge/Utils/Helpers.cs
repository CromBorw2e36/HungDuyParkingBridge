namespace HungDuyParkingBridge.Utils
{
    public static class MimeTypeHelper
    {
        public static string GetMimeType(string fileName)
        {
            return Path.GetExtension(fileName).ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".doc" or ".docx" => "application/msword",
                ".xls" or ".xlsx" => "application/vnd.ms-excel",
                ".zip" => "application/zip",
                ".rar" => "application/x-rar-compressed",
                ".7z" => "application/x-7z-compressed",
                _ => "application/octet-stream"
            };
        }

        public static bool IsCompressedFile(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext is ".zip" or ".rar" or ".7z" or ".tar" or ".gz";
        }
    }

    public static class HttpHelper
    {
        public static void AddCorsHeaders(System.Net.HttpListenerResponse response)
        {
            response.AddHeader("Access-Control-Allow-Origin", "*");
            response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            response.AddHeader("Access-Control-Allow-Headers", "*");
        }
    }
}