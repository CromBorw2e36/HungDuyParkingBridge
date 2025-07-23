using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Text;
using HungDuyParkingBridge.Models;
using HungDuyParkingBridge.Services;
using HungDuyParkingBridge.Utils;

namespace HungDuyParkingBridge.Handlers
{
    internal class FileUploadHandler
    {
        private readonly string _savePath;
        private readonly FileApiService _apiService;

        public FileUploadHandler(string savePath, FileApiService apiService)
        {
            _savePath = savePath;
            _apiService = apiService;
        }

        public async Task<bool> TryHandle(HttpListenerContext context)
        {   
            var request = context.Request;

            if (request.HttpMethod != "POST" || !request.Url?.AbsolutePath.Equals("/upload/", StringComparison.OrdinalIgnoreCase) == true)
                return false;

            var response = context.Response;
            HttpHelper.AddCorsHeaders(response);

            var boundary = GetBoundary(request.ContentType);
            if (string.IsNullOrEmpty(boundary))
            {
                response.StatusCode = 400;
                await response.OutputStream.WriteAsync("Missing boundary"u8.ToArray());
                response.Close();
                return true;
            }

            var multipartReader = new MultipartReader(boundary, request.InputStream);
            var section = await multipartReader.ReadNextSectionAsync();
            var uploadedFiles = new List<string>();

            while (section != null)
            {
                if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisp)
                    && contentDisp.DispositionType == "form-data"
                    && contentDisp.Name == "fileSave")
                {
                    string fileName = contentDisp.FileName.HasValue
                        ? contentDisp.FileName.Value.Trim('"')
                        : $"uploaded_{DateTime.Now.Ticks}.bin";

                    // Ensure unique filename
                    fileName = GetUniqueFileName(fileName);

                    string filePath = Path.Combine(_savePath, fileName);
                    using var fs = File.Create(filePath);
                    await section.Body.CopyToAsync(fs);

                    // Register file with API service
                    var fileInfo = new FileInfo(filePath);
                    _apiService.RegisterFile(fileName, fileInfo.Length);
                    uploadedFiles.Add(fileName);
                }

                section = await multipartReader.ReadNextSectionAsync();
            }

            response.StatusCode = 200;
            response.ContentType = "application/json";
            var responseJson = System.Text.Json.JsonSerializer.Serialize(new 
            { 
                success = true, 
                message = "Files uploaded successfully", 
                files = uploadedFiles 
            });
            await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(responseJson));
            response.Close();
            return true;
        }

        private string GetUniqueFileName(string fileName)
        {
            var baseName = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            var filePath = Path.Combine(_savePath, fileName);
            var counter = 1;

            while (File.Exists(filePath))
            {
                fileName = $"{baseName}_{counter}{extension}";
                filePath = Path.Combine(_savePath, fileName);
                counter++;
            }

            return fileName;
        }

        private static string? GetBoundary(string? contentType)
        {
            const string BoundaryKey = "boundary=";
            if (string.IsNullOrEmpty(contentType)) return null;

            foreach (var part in contentType.Split(';'))
            {
                var trimmed = part.Trim();
                if (trimmed.StartsWith(BoundaryKey))
                    return trimmed[BoundaryKey.Length..];
            }

            return null;
        }
    }
}