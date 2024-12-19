using Azure.Storage.Files.Shares;

namespace Azure.Storage.Samples.API.Services.FileShare;

public interface IFileShareService
{

    Task<ShareDirectoryClient> CreateOrGetDirectoryAsync(string fileShareName, string directoryName);
    Task<ShareFileClient> CreateOrGetFileAsync(string shareName, string directoryName, string fileName, long size);
    Task<ShareClient> CreateOrGetShareAsync(string shareName);
    Task<Stream?> DownloadFileAsync(string shareName, string directoryName, string fileName);
    Task UploadFileAsync(string shareName, string directoryName, string fileName, Stream file);
    Task UploadFileRangeAsync(string shareName, string directoryName, string fileName, long offset, long dataLength, Stream file);
    Task<(bool, ShareDirectoryClient?)> GetIsExistsDirectoryAsync(string shareName, string directoryName);
    Task<(bool, ShareClient?)> GetIsExistsShareAsync(string shareName);
    Task<(bool, ShareFileClient?)> GetIsExitstFileAsync(string shareName, string directoryName, string fileName);
    Task<bool> DeleteShareIfExistsAsync(string shareName);
    Task<bool> DeleteDirectoryIfExistsAsync(string shareName, string directoryName);
    Task<bool> DeleteFileIfExistsAsync(string shareName, string directoryName, string fileName);

}
