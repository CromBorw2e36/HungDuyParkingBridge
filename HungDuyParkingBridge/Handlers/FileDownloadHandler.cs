using System.Net;
using HungDuyParkingBridge.Utils;
using HungDuyParkingBridge.Services;

namespace HungDuyParkingBridge.Handlers
{
    internal class FileDownloadHandler
    {
        private readonly string _savePath;
        private WebSocketService? _webSocketService;

        public FileDownloadHandler(string savePath)
        {
            _savePath = savePath;
        }

        public void SetWebSocketService(WebSocketService webSocketService)
        {
            _webSocketService = webSocketService;
        }

        public async Task<bool> TryHandle(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod != "GET" || !request.Url?.AbsolutePath.StartsWith("/files/") == true)
                return false;

            HttpHelper.AddCorsHeaders(response);

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
            response.ContentType = MimeTypeHelper.GetMimeType(fileName);
            response.ContentLength64 = fileBytes.Length;
            await response.OutputStream.WriteAsync(fileBytes);
            response.Close();

            // Send WebSocket notification for download
            if (_webSocketService != null)
            {
                await _webSocketService.BroadcastFileNotificationAsync(fileName, "downloaded", fileBytes.Length);
            }

            return true;
        }
    }
}