using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HungDuyParkingBridge
{
    internal class FileDownloadHandler
    {
        private readonly string _savePath;

        public FileDownloadHandler(string savePath)
        {
            _savePath = savePath;
        }

        public async Task<bool> TryHandle(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod != "GET" || !request.Url?.AbsolutePath.StartsWith("/files/") == true)
                return false;

            AddCorsHeaders(response);

            string fileName = WebUtility.UrlDecode(request.Url.AbsolutePath["/files/".Length..]);
            string filePath = Path.Combine(_savePath, fileName);

            if (!File.Exists(filePath))
            {
                response.StatusCode = 404;
                await response.OutputStream.WriteAsync("Not Found"u8.ToArray());
                response.Close();
                return true;
            }

            byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
            response.StatusCode = 200;
            response.ContentType = GetMimeType(fileName);
            response.ContentLength64 = fileBytes.Length;
            await response.OutputStream.WriteAsync(fileBytes);
            response.Close();
            return true;
        }

        private static void AddCorsHeaders(HttpListenerResponse response)
        {
            response.AddHeader("Access-Control-Allow-Origin", "*");
            response.AddHeader("Access-Control-Allow-Methods", "GET, OPTIONS");
            response.AddHeader("Access-Control-Allow-Headers", "*");
        }

        private static string GetMimeType(string fileName)
        {
            return Path.GetExtension(fileName).ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
}
