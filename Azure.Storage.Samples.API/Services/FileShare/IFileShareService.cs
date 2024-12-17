namespace Azure.Storage.Samples.API.Services.FileShare
{
    public interface IFileShareService
    {
        Task CreateFileIfNotExistsAsync(string fileShareName, string directoryName, string fileName, , long size);

        Task CreateDirectoryIfNotExistsAsync(string fileShareName, string directoryName);

        Task CreateFileShareIfNotExistsAsync(string fileShareName);

        Task UploadFileAsync(string fileShareName, string directoryName, Stream file);

        Task UploadFileRangeAsync(
            string fileShareName,
            string directoryName,
            long offset,
            long dataLength,
            Stream file);

        Task<Stream> DownloadFileAsync(string fileShareName, string directoryName, string fileName);

        Task<bool> DeleteFileIfExistsAsync(string fileShareName, string directoryName, string fileName);

    }
}
