using Google.Apis.Drive.v3.Data;
using Google.Drive.Query.Integration.Query.File;
using Microsoft.AspNetCore.Mvc;
using GoogleFile = Google.Apis.Drive.v3.Data.File;

namespace Google.Drive.Query.Integration.Interface
{ 
    public interface IIntegration
    {
        /// <summary>
        /// Create a folder on the given Drive Id <br/>
        /// If the 'parentId' is null, the folder will be created on the drive's root
        /// </summary>
        /// <param name="driveId">Drive Id where the folder will be created</param>
        /// <param name="folderName">Folder's name</param>
        /// <param name="parentId">Parent Id</param>
        /// <returns>Details of the created folder</returns>
        Task<ActionResult<GoogleFile>> CreateFolderAsync(string driveId, string folderName, string? parentId = null);

        /// <summary>
        /// Upload the file Stream file<br/>
        /// If the 'parentId' is null, o file will be created on the drive's root
        /// </summary>
        /// <param name="driveId">Drive ID where the file will be sent.</param>
        /// <param name="file">File's Stream</param>
        /// <param name="fileName">File's Name</param>
        /// <param name="parentId">Folder's Id where the file will be sent to</param>
        /// <param name="fileDescription">File description</param>
        /// <returns>
        /// The details of the file that was sent
        /// </returns>
        Task<ActionResult<GoogleFile>> UploadFileAsync(string driveId, Stream file, string fileName, string? parentId = null, string? fileDescription = null);

        /// <summary>
        /// Search files/folders<br/>
        /// </summary>
        /// <param name="terms">The FileTerms class with the properties to filter the search</param>
        /// <param name="driveId">Drive's Id where the files will be searched</param>
        /// <param name="fields">Files that contains the string on their names</param>
        /// <returns>
        /// </returns>
        Task<ActionResult<List<GoogleFile>>> ListAsync(FileTerms terms, string? fields = null, string? driveId = null);

        /// <summary>
        /// Search files/folders<br/>
        /// </summary>
        /// <param name="query">Query to filter the search</param>
        /// <param name="driveId">Drive's Id where the files will be searched</param>
        /// <param name="fields">Files that contains the string on their names</param>
        /// <returns>
        /// </returns>
        Task<ActionResult<List<GoogleFile>>> ListAsync(string? query = null, string? fields = null, string? driveId = null);

        /// <summary>
        /// Search all drives that are been shared with the service account being used by the application
        /// </summary>
        /// <returns>
        /// All the drives details
        /// </returns>
        Task<ActionResult<TeamDriveList>> GetAllTeamDrivesAsync();

        /// <summary>
        /// Download a file from the drive
        /// </summary>
        /// <param name="fileId">Files's ID to be downloaded</param>
        /// <returns>
        /// The file in a byte array
        /// </returns>
        Task<ActionResult<byte[]>> DownloadFileAsync(string fileId);

        /// <summary>
        /// Gets a file's metadata by ID
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        Task<ActionResult<GoogleFile>> GetFileInfoAsync(string fileId, string? fields = null);
    }
}
