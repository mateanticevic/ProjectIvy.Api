using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;

namespace ProjectIvy.Data.Services.AzureStorage;

public class AzureStorageHelper : IAzureStorageHelper
{
    private readonly CloudStorageAccount _account;
    private readonly CloudFileClient _client;

    public AzureStorageHelper(string connectionString)
    {
        _account = CloudStorageAccount.Parse(connectionString);
        _client = _account.CreateCloudFileClient();
    }

    public async Task DeleteFile(string fileName)
    {
        var parts = fileName.Split('/');

        var share = _client.GetShareReference(parts[0]);
        var directory = share.GetRootDirectoryReference();

        var file = directory.GetFileReference(parts[1]);

        await file.DeleteAsync();
    }

    public async Task<byte[]> GetFile(string fileName)
    {
        var parts = fileName.Split('/');

        var share = _client.GetShareReference(parts[0]);
        var directory = share.GetRootDirectoryReference();

        var file = directory.GetFileReference(parts[1]);

        using (var stream = new MemoryStream())
        {
            await file.DownloadToStreamAsync(stream);
            return stream.ToArray();
        }
    }

    public async Task UploadFile(string fileName, byte[] fileData)
    {
        var parts = fileName.Split('/');

        var share = _client.GetShareReference(parts[0]);
        var directory = share.GetRootDirectoryReference();

        var file = directory.GetFileReference(parts[1]);

        await file.UploadFromByteArrayAsync(fileData, 0, fileData.Length);
    }
}
