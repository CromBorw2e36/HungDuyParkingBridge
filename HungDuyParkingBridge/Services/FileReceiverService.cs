using System.Net;
using System.Diagnostics;
using HungDuyParkingBridge.Handlers;
using HungDuyParkingBridge.Services;

namespace HungDuyParkingBridge.Services
{
    internal class FileReceiverService
    {
        private readonly HttpListener _listener = new();
        private readonly string _savePath = @"C:\HungDuyParkingReceivedFiles";
        private FileUploadHandler _uploadHandler;
        private FileDownloadHandler _downloadHandler;
        private FileApiService _apiService;
        private WebSocketService _webSocketService;

        public async Task Start()
        {
            Directory.CreateDirectory(_savePath);
            _listener.Prefixes.Add("http://localhost:5000/");
            _listener.Start();

            _apiService = new FileApiService(_savePath);
            _uploadHandler = new FileUploadHandler(_savePath, _apiService);
            _downloadHandler = new FileDownloadHandler(_savePath);
            
            // Start WebSocket service on port 5001
            _webSocketService = new WebSocketService("http://localhost:5001/");
            await _webSocketService.StartAsync();

            // Pass WebSocket service to handlers for notifications
            _uploadHandler.SetWebSocketService(_webSocketService);
            _downloadHandler.SetWebSocketService(_webSocketService);

            Task.Run(async () =>
            {
                try
                {
                    while (_listener.IsListening)
                    {
                        var context = await _listener.GetContextAsync();
                        var request = context.Request;

                        // OPTIONS preflight
                        if (request.HttpMethod == "OPTIONS")
                        {
                            var resp = context.Response;
                            resp.AddHeader("Access-Control-Allow-Origin", "*");
                            resp.AddHeader("Access-Control-Allow-Methods", "POST, GET, PUT, DELETE, OPTIONS");
                            resp.AddHeader("Access-Control-Allow-Headers", "*");
                            resp.StatusCode = 200;
                            resp.Close();
                            continue;
                        }

                        // Try API service first
                        if (await _apiService.TryHandle(context)) continue;
                        if (await _uploadHandler.TryHandle(context)) continue;
                        if (await _downloadHandler.TryHandle(context)) continue;

                        context.Response.StatusCode = 404;
                        await context.Response.OutputStream.WriteAsync("Unknown endpoint"u8.ToArray());
                        context.Response.Close();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[HttpListener Error] " + ex.Message);
                }
            });

            Debug.WriteLine("[FileReceiver] HTTP Server started on http://localhost:5000");
            Debug.WriteLine("[FileReceiver] WebSocket Server started on http://localhost:5001");
        }

        public async Task Stop()
        {
            _listener.Stop();
            if (_webSocketService != null)
            {
                await _webSocketService.StopAsync();
            }
        }

        public async Task NotifyFileUploaded(string fileName, long fileSize)
        {
            if (_webSocketService != null)
            {
                await _webSocketService.BroadcastFileNotificationAsync(fileName, "uploaded", fileSize);
            }
        }

        public async Task NotifyFileDownloaded(string fileName, long fileSize)
        {
            if (_webSocketService != null)
            {
                await _webSocketService.BroadcastFileNotificationAsync(fileName, "downloaded", fileSize);
            }
        }

        public async Task BroadcastMessage(string message)
        {
            if (_webSocketService != null)
            {
                await _webSocketService.BroadcastMessageAsync(message);
            }
        }
    }
}