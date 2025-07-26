using System.Net;
using System.Text;
using Newtonsoft.Json;
using HungDuyParkingBridge.Models;
using HungDuyParkingBridge.Utils;
using SharpCompress.Archives;

namespace HungDuyParkingBridge.Services
{
    internal class FileApiService
    {
        private readonly string _basePath;
        private readonly string _metadataPath;
        private readonly Dictionary<string, FileMetadata> _metadata;

        public FileApiService(string basePath)
        {
            _basePath = basePath;
            _metadataPath = Path.Combine(_basePath, "metadata.json");
            _metadata = LoadMetadata();
        }

        public async Task<bool> TryHandle(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            // Add CORS headers
            HttpHelper.AddCorsHeaders(response);

            // Handle OPTIONS preflight
            if (request.HttpMethod == "OPTIONS")
            {
                response.StatusCode = 200;
                response.Close();
                return true;
            }

            var path = request.Url?.AbsolutePath ?? "";

            try
            {
                return path switch
                {
                    "/api/status" when request.HttpMethod == "GET" => await HandleStatus(context),
                    "/api/health" when request.HttpMethod == "GET" => await HandleStatus(context), // Alternative endpoint
                    "/api/ping" when request.HttpMethod == "GET" => await HandleStatus(context),   // Alternative endpoint
                    "/api/files" when request.HttpMethod == "GET" => await HandleListFiles(context),
                    "/api/files" when request.HttpMethod == "POST" => await HandleCreateFile(context),
                    var p when p.StartsWith("/api/files/") && request.HttpMethod == "GET" => await HandleGetFile(context),
                    var p when p.StartsWith("/api/files/") && request.HttpMethod == "PUT" => await HandleUpdateFile(context),
                    var p when p.StartsWith("/api/files/") && request.HttpMethod == "DELETE" => await HandleDeleteFile(context),
                    "/api/files/compress" when request.HttpMethod == "POST" => await HandleCompressFiles(context),
                    "/api/files/extract" when request.HttpMethod == "POST" => await HandleExtractFiles(context),
                    "/api/folders" when request.HttpMethod == "POST" => await HandleCreateFolder(context),
                    _ => false
                };
            }
            catch (Exception ex)
            {
                await SendErrorResponse(response, $"Internal server error: {ex.Message}", 500);
                return true;
            }
        }

        private async Task<bool> HandleListFiles(HttpListenerContext context)
        {
            var response = context.Response;
            var queryParams = context.Request.QueryString;
            var parentId = queryParams.Get("parentId");

            var files = _metadata.Values
                .Where(f => f.ParentId == parentId)
                .OrderBy(f => f.IsFolder ? 0 : 1)
                .ThenBy(f => f.Name)
                .ToList();

            await SendJsonResponse(response, new ApiResponse<List<FileMetadata>>
            {
                Success = true,
                Message = "Files retrieved successfully",
                Data = files
            });

            return true;
        }

        private async Task<bool> HandleGetFile(HttpListenerContext context)
        {
            var response = context.Response;
            var fileId = ExtractFileIdFromPath(context.Request.Url?.AbsolutePath ?? "");

            if (!_metadata.TryGetValue(fileId, out var metadata))
            {
                await SendErrorResponse(response, "File not found", 404);
                return true;
            }

            if (metadata.IsFolder)
            {
                await SendJsonResponse(response, new ApiResponse<FileMetadata>
                {
                    Success = true,
                    Message = "Folder metadata retrieved",
                    Data = metadata
                });
                return true;
            }

            var filePath = Path.Combine(_basePath, metadata.Name);
            if (!File.Exists(filePath))
            {
                await SendErrorResponse(response, "Physical file not found", 404);
                return true;
            }

            var fileBytes = await File.ReadAllBytesAsync(filePath);
            response.StatusCode = 200;
            response.ContentType = metadata.MimeType;
            response.ContentLength64 = fileBytes.Length;
            response.AddHeader("Content-Disposition", $"attachment; filename=\"{metadata.Name}\"");
            await response.OutputStream.WriteAsync(fileBytes);
            response.Close();

            return true;
        }

        private async Task<bool> HandleDeleteFile(HttpListenerContext context)
        {
            var response = context.Response;
            var fileId = ExtractFileIdFromPath(context.Request.Url?.AbsolutePath ?? "");

            if (!_metadata.TryGetValue(fileId, out var metadata))
            {
                await SendErrorResponse(response, "File not found", 404);
                return true;
            }

            // Delete physical file/folder
            if (metadata.IsFolder)
            {
                var folderPath = Path.Combine(_basePath, metadata.Name);
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }
            }
            else
            {
                var filePath = Path.Combine(_basePath, metadata.Name);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            // Remove from metadata
            _metadata.Remove(fileId);
            await SaveMetadata();

            await SendJsonResponse(response, new ApiResponse<object>
            {
                Success = true,
                Message = "File deleted successfully"
            });

            return true;
        }

        private async Task<bool> HandleCreateFolder(HttpListenerContext context)
        {
            var response = context.Response;
            var requestBody = await ReadRequestBody(context.Request);
            var folderData = JsonConvert.DeserializeAnonymousType(requestBody, new { name = "", parentId = (string?)null });

            if (string.IsNullOrEmpty(folderData?.name))
            {
                await SendErrorResponse(response, "Folder name is required", 400);
                return true;
            }

            var folderId = Guid.NewGuid().ToString();
            var folderPath = Path.Combine(_basePath, folderData.name);
            
            Directory.CreateDirectory(folderPath);

            var metadata = new FileMetadata
            {
                Id = folderId,
                Name = folderData.name,
                Size = 0,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                MimeType = "application/vnd.google-apps.folder",
                IsCompressed = false,
                ParentId = folderData.parentId,
                IsFolder = true
            };

            _metadata[folderId] = metadata;
            await SaveMetadata();

            await SendJsonResponse(response, new ApiResponse<FileMetadata>
            {
                Success = true,
                Message = "Folder created successfully",
                Data = metadata
            });

            return true;
        }

        private async Task<bool> HandleCompressFiles(HttpListenerContext context)
        {
            var response = context.Response;
            var requestBody = await ReadRequestBody(context.Request);
            var compressData = JsonConvert.DeserializeAnonymousType(requestBody, new { fileIds = new string[0], archiveName = "" });

            if (compressData?.fileIds == null || compressData.fileIds.Length == 0)
            {
                await SendErrorResponse(response, "File IDs are required", 400);
                return true;
            }

            var archiveName = !string.IsNullOrEmpty(compressData.archiveName) 
                ? compressData.archiveName 
                : $"archive_{DateTime.Now:yyyyMMdd_HHmmss}.zip";

            if (!archiveName.EndsWith(".zip"))
                archiveName += ".zip";

            var archivePath = Path.Combine(_basePath, archiveName);
            
            // Use SharpCompress to create archive
            using (var archive = SharpCompress.Archives.Zip.ZipArchive.Create())
            {
                foreach (var fileId in compressData.fileIds)
                {
                    if (_metadata.TryGetValue(fileId, out var metadata) && !metadata.IsFolder)
                    {
                        var filePath = Path.Combine(_basePath, metadata.Name);
                        if (File.Exists(filePath))
                        {
                            archive.AddEntry(metadata.Name, filePath);
                        }
                    }
                }

                using var fileStream = File.Create(archivePath);
                archive.SaveTo(fileStream, SharpCompress.Common.CompressionType.Deflate);
            }

            // Create metadata for the archive
            var archiveId = Guid.NewGuid().ToString();
            var archiveInfo = new FileInfo(archivePath);
            var archiveMetadata = new FileMetadata
            {
                Id = archiveId,
                Name = archiveName,
                Size = archiveInfo.Length,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                MimeType = "application/zip",
                IsCompressed = true,
                ParentId = null,
                IsFolder = false
            };

            _metadata[archiveId] = archiveMetadata;
            await SaveMetadata();

            await SendJsonResponse(response, new ApiResponse<FileMetadata>
            {
                Success = true,
                Message = "Files compressed successfully",
                Data = archiveMetadata
            });

            return true;
        }

        private async Task<bool> HandleExtractFiles(HttpListenerContext context)
        {
            var response = context.Response;
            var requestBody = await ReadRequestBody(context.Request);
            var extractData = JsonConvert.DeserializeAnonymousType(requestBody, new { fileId = "", extractPath = "" });

            if (string.IsNullOrEmpty(extractData?.fileId))
            {
                await SendErrorResponse(response, "File ID is required", 400);
                return true;
            }

            if (!_metadata.TryGetValue(extractData.fileId, out var metadata))
            {
                await SendErrorResponse(response, "File not found", 404);
                return true;
            }

            var archivePath = Path.Combine(_basePath, metadata.Name);
            if (!File.Exists(archivePath))
            {
                await SendErrorResponse(response, "Archive file not found", 404);
                return true;
            }

            var extractDir = !string.IsNullOrEmpty(extractData.extractPath) 
                ? Path.Combine(_basePath, extractData.extractPath)
                : Path.Combine(_basePath, $"extracted_{DateTime.Now:yyyyMMdd_HHmmss}");

            Directory.CreateDirectory(extractDir);

            // Extract using SharpCompress
            using var archive = SharpCompress.Archives.ArchiveFactory.Open(archivePath);
            foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
            {
                entry.WriteToDirectory(extractDir, new SharpCompress.Common.ExtractionOptions()
                {
                    ExtractFullPath = true,
                    Overwrite = true
                });
            }

            await SendJsonResponse(response, new ApiResponse<object>
            {
                Success = true,
                Message = $"Files extracted to {Path.GetFileName(extractDir)}"
            });

            return true;
        }

        private async Task<bool> HandleCreateFile(HttpListenerContext context)
        {
            // This will be handled by the existing FileUploadHandler
            // but we'll update metadata here
            return false; // Let other handlers process upload
        }

        private async Task<bool> HandleUpdateFile(HttpListenerContext context)
        {
            var response = context.Response;
            var fileId = ExtractFileIdFromPath(context.Request.Url?.AbsolutePath ?? "");
            var requestBody = await ReadRequestBody(context.Request);
            var updateData = JsonConvert.DeserializeAnonymousType(requestBody, new { name = "" });

            if (!_metadata.TryGetValue(fileId, out var metadata))
            {
                await SendErrorResponse(response, "File not found", 404);
                return true;
            }

            if (!string.IsNullOrEmpty(updateData?.name) && updateData.name != metadata.Name)
            {
                var oldPath = Path.Combine(_basePath, metadata.Name);
                var newPath = Path.Combine(_basePath, updateData.name);

                if (metadata.IsFolder && Directory.Exists(oldPath))
                {
                    Directory.Move(oldPath, newPath);
                }
                else if (!metadata.IsFolder && File.Exists(oldPath))
                {
                    File.Move(oldPath, newPath);
                }

                metadata.Name = updateData.name;
                metadata.ModifiedDate = DateTime.Now;
                await SaveMetadata();
            }

            await SendJsonResponse(response, new ApiResponse<FileMetadata>
            {
                Success = true,
                Message = "File updated successfully",
                Data = metadata
            });

            return true;
        }

        private async Task<bool> HandleStatus(HttpListenerContext context)
        {
            var response = context.Response;
            
            // Create comprehensive status response
            var statusData = new
            {
                status = true,
                server = "HungDuyParkingBridge",
                version = HDParkingConst.version,
                timestamp = DateTime.UtcNow.ToString("o"),
                uptime = GetUptime(),
                endpoints = new string[]
                {
                    "GET /api/status",
                    "GET /api/health", 
                    "GET /api/ping",
                    "GET /api/files",
                    "POST /api/files",
                    "POST /upload/",
                    "GET /download/{filename}"
                }
            };

            var statusResponse = new ApiResponse<object>
            {
                Success = true,
                Message = "Server is running",
                Data = statusData
            };

            await SendJsonResponse(response, statusResponse);
            return true;
        }

        private string GetUptime()
        {
            try
            {
                var uptime = DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime;
                return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
            }
            catch
            {
                return "Unknown";
            }
        }

        private string ExtractFileIdFromPath(string path)
        {
            return path.Split('/').LastOrDefault() ?? "";
        }

        private async Task<string> ReadRequestBody(HttpListenerRequest request)
        {
            using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
            return await reader.ReadToEndAsync();
        }

        private async Task SendJsonResponse<T>(HttpListenerResponse response, T data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            
            response.StatusCode = 200;
            response.ContentType = "application/json";
            response.ContentLength64 = jsonBytes.Length;
            await response.OutputStream.WriteAsync(jsonBytes);
            response.Close();
        }

        private async Task SendErrorResponse(HttpListenerResponse response, string message, int statusCode = 400)
        {
            var errorResponse = new ApiResponse<object>
            {
                Success = false,
                Message = message
            };

            var json = JsonConvert.SerializeObject(errorResponse);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            
            response.StatusCode = statusCode;
            response.ContentType = "application/json";
            response.ContentLength64 = jsonBytes.Length;
            await response.OutputStream.WriteAsync(jsonBytes);
            response.Close();
        }

        private Dictionary<string, FileMetadata> LoadMetadata()
        {
            if (!File.Exists(_metadataPath))
                return new Dictionary<string, FileMetadata>();

            try
            {
                var json = File.ReadAllText(_metadataPath);
                return JsonConvert.DeserializeObject<Dictionary<string, FileMetadata>>(json) 
                       ?? new Dictionary<string, FileMetadata>();
            }
            catch
            {
                return new Dictionary<string, FileMetadata>();
            }
        }

        private async Task SaveMetadata()
        {
            var json = JsonConvert.SerializeObject(_metadata, Formatting.Indented);
            await File.WriteAllTextAsync(_metadataPath, json);
        }

        public void RegisterFile(string fileName, long size)
        {
            var fileId = Guid.NewGuid().ToString();
            var metadata = new FileMetadata
            {
                Id = fileId,
                Name = fileName,
                Size = size,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                MimeType = MimeTypeHelper.GetMimeType(fileName),
                IsCompressed = MimeTypeHelper.IsCompressedFile(fileName),
                ParentId = null,
                IsFolder = false
            };

            _metadata[fileId] = metadata;
            Task.Run(SaveMetadata);
        }
    }
}