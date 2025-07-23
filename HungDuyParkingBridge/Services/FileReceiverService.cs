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

        public void Start()
        {
            Directory.CreateDirectory(_savePath);
            _listener.Prefixes.Add("http://localhost:5000/");
            _listener.Start();

            _apiService = new FileApiService(_savePath);
            _uploadHandler = new FileUploadHandler(_savePath, _apiService);
            _downloadHandler = new FileDownloadHandler(_savePath);

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
        }

        public void Stop() => _listener.Stop();
    }
}