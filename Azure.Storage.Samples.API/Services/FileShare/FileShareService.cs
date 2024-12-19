
using Azure.Storage.Files.Shares;
using Azure.Storage.Samples.API.Options;
using Microsoft.Extensions.Options;

namespace Azure.Storage.Samples.API.Services.FileShare;

public class FileShareService : IFileShareService
{

    private readonly ShareServiceClient _shareServiceClient;

    public FileShareService(IOptions<AzureOptions> options)
    {
        _shareServiceClient = new ShareServiceClient(options.Value.ConnectionString);
    }

    public async Task<ShareDirectoryClient> CreateOrGetDirectoryAsync(string fileShareName, string directoryName)
    {
        var shareClient = await CreateOrGetShareAsync(fileShareName);
        var directoryClient = shareClient.GetDirectoryClient(directoryName);
        await directoryClient.CreateIfNotExistsAsync();
        return directoryClient;
    }

    public async Task<ShareFileClient> CreateOrGetFileAsync(string shareName, string directoryName, string fileName, long size)
    {
        var directoryClient = await CreateOrGetDirectoryAsync(shareName, directoryName);
        var fileClient = directoryClient.GetFileClient(fileName);

        if (!await fileClient.ExistsAsync()) await fileClient.CreateAsync(size);

        return fileClient;

    }

    public async Task<ShareClient> CreateOrGetShareAsync(string fileShareName)
    {

        var shareClient = _shareServiceClient.GetShareClient(fileShareName);
        await shareClient.CreateIfNotExistsAsync();

        return shareClient;

    }

    public async Task<bool> DeleteFileIfExistsAsync(string shareName, string directoryName, string fileName)
    {


        var response = await GetIsExitstFileAsync(shareName, directoryName, fileName);

        if (!response.Item1) return false;

        await response.Item2!.DeleteAsync();
        return true;
    }

    public async Task<Stream?> DownloadFileAsync(string fileShareName, string directoryName, string fileName)
    {
        var fileResponse = await GetIsExitstFileAsync(fileShareName, directoryName, fileName);

        if (!fileResponse.Item1)
            return null;

        return await fileResponse.Item2!.OpenReadAsync();


    }
    public async Task UploadFileAsync(string fileShareName, string directoryName, string fileName, Stream file)
    {
        var fileClient = await CreateOrGetFileAsync(fileShareName, directoryName, fileName, file.Length);



        await fileClient.UploadAsync(file);
    }

    public async Task UploadFileRangeAsync(string fileShareName, string directoryName, string fileName, long offset, long dataLength, Stream file)
    {
        var fileClient = await CreateOrGetFileAsync(fileShareName, directoryName, fileName, file.Length);

        await fileClient.SetHttpHeadersAsync(new Files.Shares.Models.ShareFileSetHttpHeadersOptions
        {
            NewSize = dataLength + (await fileClient.GetPropertiesAsync()).Value.ContentLength,
        });

        await fileClient.UploadRangeAsync(new HttpRange(offset, dataLength), file);
    }

    public async Task<(bool, ShareDirectoryClient?)> GetIsExistsDirectoryAsync(string shareName, string directoryName)
    {
        var responseShare = await GetIsExistsShareAsync(shareName);

        if (!responseShare.Item1) return (false, null);


        var directoryClient = responseShare.Item2!.GetDirectoryClient(directoryName);

        try
        {
            return await directoryClient.ExistsAsync()
           ? (true, directoryClient)
           : (false, null);
        }
        catch (RequestFailedException ex)
        {
            Console.WriteLine(ex);

            return (false, null);
        }

    }

    public async Task<(bool, ShareClient?)> GetIsExistsShareAsync(string shareName)
    {
        var shareClient = _shareServiceClient.GetShareClient(shareName);

        try
        {
            return await shareClient.ExistsAsync()
           ? (true, shareClient)
           : (false, null);
        }
        catch (RequestFailedException ex)
        {
            Console.WriteLine(ex);

            return (false, null);
        }

    }

    public async Task<(bool, ShareFileClient?)> GetIsExitstFileAsync(string shareName, string directoryName, string fileName)
    {
        var responseDirectory = await GetIsExistsDirectoryAsync(shareName, directoryName);

        if (!responseDirectory.Item1) return (false, null);

        var fileClient = responseDirectory.Item2!.GetFileClient(fileName);

        try
        {
            return await fileClient.ExistsAsync()
           ? (true, fileClient)
           : (false, null);
        }
        catch (RequestFailedException ex)
        {
            Console.WriteLine(ex);

            return (false, null);
        }

    }


    public async Task<bool> DeleteShareIfExistsAsync(string shareName)
    {
        var shareResponse = await GetIsExistsShareAsync(shareName);

        if (!shareResponse.Item1)
            return false;

        try
        {
            await shareResponse.Item2!.DeleteAsync();
        }
        catch (RequestFailedException ex)
        {
            Console.WriteLine(ex);
            return false;
        }

        return true;

    }

    public async Task<bool> DeleteDirectoryIfExistsAsync(string shareName, string directoryName)
    {
        var directoryResponse = await GetIsExistsDirectoryAsync(shareName, directoryName);

        if (!directoryResponse.Item1)
            return false;

        await directoryResponse.Item2!.DeleteAsync();


        return true;
    }


}