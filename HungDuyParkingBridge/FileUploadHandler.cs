using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace HungDuyParkingBridge
{
    internal class FileUploadHandler
    {
        private readonly string _savePath;

        public FileUploadHandler(string savePath)
        {
            _savePath = savePath;
        }

        public async Task<bool> TryHandle(HttpListenerContext context)
        {   
            var request = context.Request;

            if (request.HttpMethod != "POST" || !request.Url?.AbsolutePath.Equals("/upload/", StringComparison.OrdinalIgnoreCase) == true)
                return false;

            var response = context.Response;
            AddCorsHeaders(response);

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

            while (section != null)
            {
                if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisp)
                    && contentDisp.DispositionType == "form-data"
                    && contentDisp.Name == "fileSave")
                {
                    string fileName = contentDisp.FileName.HasValue
                        ? contentDisp.FileName.Value.Trim('"')
                        : $"uploaded_{DateTime.Now.Ticks}.bin";

                    string filePath = Path.Combine(_savePath, fileName);
                    using var fs = File.Create(filePath);
                    await section.Body.CopyToAsync(fs);
                }

                section = await multipartReader.ReadNextSectionAsync();
            }

            response.StatusCode = 200;
            await response.OutputStream.WriteAsync("OK"u8.ToArray());
            response.Close();
            return true;
        }

        private static void AddCorsHeaders(HttpListenerResponse response)
        {
            response.AddHeader("Access-Control-Allow-Origin", "*");
            response.AddHeader("Access-Control-Allow-Methods", "POST, OPTIONS");
            response.AddHeader("Access-Control-Allow-Headers", "*");
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
