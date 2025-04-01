using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using GoogleFile = Google.Apis.Drive.v3.Data.File;
using Google.Apis.Download;
using Google.Drive.Query.Integration.Business;
using Google.Drive.Query.Integration.Connection;
using Google.Drive.Query.Integration.Interface;
using Google.Drive.Query.Integration.Request;
using Google.Drive.Query.Integration.Query.File;
using Microsoft.AspNetCore.Mvc;
using static Google.Apis.Drive.v3.DriveBaseServiceRequest<Google.Apis.Drive.v3.Data.File>;

namespace Google.Drive.Query.Integration
{
    public class Integration : IIntegration
    {
        private DriveService service { get; set; }
        public Integration(string applicationName, string credentialFilePath, string? emailServiceAccount = null, string? password = null)
        {
            service = ConnectionServiceAccount.CreateConnection(applicationName, credentialFilePath, emailServiceAccount, password);
        }

        public Integration(DriveService service)
        {
            this.service = service;
        }   

        public async Task<ActionResult<GoogleFile>> CreateFolderAsync(string driveId, string folderName, string? parentId = null)
        {
            try
            {
                if (!String.IsNullOrEmpty(folderName))
                {
                    GoogleFile driveFolder = new()
                    {
                        DriveId = driveId,
                        Name = folderName,
                        MimeType = "application/vnd.google-apps.folder"
                    };

                    if (!string.IsNullOrEmpty(parentId))
                    {
                        driveFolder.Parents = new string[] { parentId };
                    }

                    var command = service.Files.Create(driveFolder);
                    command.SupportsAllDrives = true;

                    var file = await command.ExecuteAsync();
                    return new OkObjectResult(file);
                }
                else
                {
                    throw new Exception("Param folderName is null or empty");
                }
            }
            catch (GoogleApiException e)
            {
                return new StatusCodeResult((int)e.HttpStatusCode);
            }
        }

        public async Task<ActionResult<GoogleFile>> UploadFileAsync(string driveId, Stream file, string fileName, string? parentId = null, string? fileDescription = null)
        {
            try
            {
                string fileMime = GoogleDriveBusiness.GetMimeType(fileName);
                GoogleFile driveFile = new()
                {
                    DriveId = driveId,
                    Name = fileName,
                    Description = fileDescription,
                    MimeType = fileMime

                };

                if (!String.IsNullOrEmpty(parentId))
                {
                    driveFile.Parents = new string[] { parentId };
                } 

                var request = service.Files.Create(driveFile, file, fileMime);
                request.Fields = "id";
                request.SupportsAllDrives = true;

                var response = await request.UploadAsync();
                if (response.Status != Google.Apis.Upload.UploadStatus.Completed)
                    throw response.Exception;

                return new OkObjectResult(request.ResponseBody);
            }
            catch (GoogleApiException e)
            {
                return new StatusCodeResult((int)e.HttpStatusCode);
            }
        }

        public async Task<ActionResult<List<GoogleFile>>> ListAsync(FileTerms terms, string? fields = null, string? driveId = null)
        {
            try
            {
                CustomListRequest request = new(service);
                request.Fields = fields;
                request.Terms = terms;
                request.SupportsAllDrives = true;
                if (driveId != null)
                {
                    request.DriveId = driveId;
                    request.IncludeItemsFromAllDrives = true;
                    request.Corpora = "drive";
                }

                var result = new List<GoogleFile>();
                string? pageToken = null;
                do
                {
                    request.PageToken = pageToken;
                    var filesResult = await request.ExecuteAsync();
                    var files = filesResult.Files;
                    pageToken = filesResult.NextPageToken;
                    result.AddRange(files);
                }
                while (pageToken != null);

                return new OkObjectResult(result);
            }
            catch (GoogleApiException e)
            {
                return new StatusCodeResult((int)e.HttpStatusCode);
            }
        }

        public async Task<ActionResult<List<GoogleFile>>> ListAsync(string? query = null, string? fields = null, string? driveId = null)
        {
            try
            {
                var request = service.Files.List();
                if (fields == null)
                    fields = "nextPageToken, files(id, name, size, mimeType, parents, createdTime, modifiedTime, trashed, version, driveId)";
                request.Fields = fields;
                request.Q = query;
                request.SupportsAllDrives = true;
                if(driveId != null)
                {
                    request.DriveId = driveId;
                    request.IncludeItemsFromAllDrives = true;
                    request.Corpora = "drive";
                }
                    
                var result = new List<GoogleFile>();
                string? pageToken = null;
                do
                {
                    request.PageToken = pageToken;
                    var filesResult = await request.ExecuteAsync();
                    var files = filesResult.Files;
                    pageToken = filesResult.NextPageToken;
                    result.AddRange(files);
                }
                while (pageToken != null);

                return new OkObjectResult(result);
            }
            catch (GoogleApiException e)
            {
                return new StatusCodeResult((int)e.HttpStatusCode);
            }
        }

        public async Task<ActionResult<DriveList>> GetSharedDrivesAsync()
        {
            try
            {
                var request = service.Drives.List();
                request.PageSize = 100;
                return new OkObjectResult(await request.ExecuteAsync());
            }
            catch (GoogleApiException e)
            {
                return new StatusCodeResult((int)e.HttpStatusCode);
            }
        }

        public async Task<ActionResult<TeamDriveList>> GetAllTeamDrivesAsync()
        {
            try
            {
                var request = service.Teamdrives.List();
                request.PageSize = 100;
                return new OkObjectResult(await request.ExecuteAsync());
            }
            catch (GoogleApiException e)
            {
                return new StatusCodeResult((int)e.HttpStatusCode);
            }
        }

        public async Task<ActionResult<byte[]>> DownloadFileAsync(string fileId, bool acknowledgeAbuse = false, AltEnum altEnum = AltEnum.Media)
        {
            try { 
                var request = service.Files.Get(fileId);
                request.SupportsAllDrives = true;
                request.AcknowledgeAbuse = acknowledgeAbuse;
                request.Alt = altEnum;

                using (MemoryStream stream = new MemoryStream())
                {
                    request.MediaDownloader.ProgressChanged +=
                        progress =>
                        {
                            if (progress.Status == DownloadStatus.Failed)
                            {
                                throw new Exception("Download failed.");
                            }
                        };
                    await request.DownloadAsync(stream);

                    var fileBytes = stream.ToArray();
                    return new OkObjectResult(fileBytes);
                }
            }
            catch (GoogleApiException e)
            {
                return new StatusCodeResult((int)e.HttpStatusCode);
            }
        }

        public async Task<ActionResult> DownloadWriteFileAsync(string fileId, string fileNamePath, bool acknowledgeAbuse = false, AltEnum altEnum = AltEnum.Media)
        {
            try
            {
                var path = fileNamePath.Substring(0, fileNamePath.LastIndexOf("\\"));
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var request = service.Files.Get(fileId);
                request.SupportsAllDrives = true;
                request.AcknowledgeAbuse = acknowledgeAbuse;
                request.Alt = altEnum;

                using (FileStream stream = new(fileNamePath, FileMode.Create))
                {
                    request.MediaDownloader.ProgressChanged +=
                        progress =>
                        {
                            if (progress.Status == DownloadStatus.Failed)
                            {
                                throw new Exception("Download failed.");
                            }
                        };
                    await request.DownloadAsync(stream);
                    stream.Flush();
                    return new OkResult();
                }
            }
            catch (GoogleApiException e)
            {
                return new ObjectResult(e);
            }
        }

        public async Task<ActionResult<byte[]>> ExportFileAsync(string fileId, string mimeType, bool acknowledgeAbuse = false, AltEnum altEnum = AltEnum.Media)
        {
            try
            {
                var request = service.Files.Export(fileId, mimeType);

                using (MemoryStream stream = new MemoryStream())
                {
                    request.MediaDownloader.ProgressChanged +=
                        progress =>
                        {
                            if (progress.Status == DownloadStatus.Failed)
                            {
                                throw new Exception("Download failed.");
                            }
                        };
                    await request.DownloadAsync(stream);

                    var fileBytes = stream.ToArray();
                    return new OkObjectResult(fileBytes);
                }
            }
            catch (GoogleApiException e)
            {
                return new StatusCodeResult((int)e.HttpStatusCode);
            }
        }

        public async Task<ActionResult> ExportWriteFileAsync(string fileId, string mimeType, string fileNamePath, bool acknowledgeAbuse = false, AltEnum altEnum = AltEnum.Media)
        {
            try
            {
                var path = fileNamePath.Substring(0, fileNamePath.LastIndexOf("\\"));
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var request = service.Files.Export(fileId, mimeType);

                using (FileStream stream = new FileStream(fileNamePath, FileMode.Create))
                {
                    request.MediaDownloader.ProgressChanged +=
                        progress =>
                        {
                            if (progress.Status == DownloadStatus.Failed)
                            {
                                throw new Exception("Download failed.");
                            }
                        };
                    await request.DownloadAsync(stream);

                    stream.Flush();
                    return new OkResult();
                }
            }
            catch (GoogleApiException e)
            {
                return new ObjectResult(e);
            }
        }

        public async Task<ActionResult<GoogleFile>> GetFileInfoAsync(string fileId, string? fields = null)
        {
            try
            {
                var request = service.Files.Get(fileId);
                request.SupportsAllDrives = true;
                if (fields == null)
                    fields = "id, name, size, mimeType, parents, createdTime, modifiedTime";
                request.Fields = fields;

                return new OkObjectResult(await request.ExecuteAsync());
            }
            catch (GoogleApiException e)
            {
                return new StatusCodeResult((int)e.HttpStatusCode);
            }
        }
    }
}
