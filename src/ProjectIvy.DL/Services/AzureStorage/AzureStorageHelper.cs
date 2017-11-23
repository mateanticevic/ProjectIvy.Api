using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System.Threading.Tasks;

namespace ProjectIvy.DL.Services.AzureStorage
{
    public class AzureStorageHelper : IAzureStorageHelper
    {
        private readonly CloudStorageAccount _account;
        private readonly CloudFileClient _client;

        public AzureStorageHelper(Common.Configuration.Services.AzureStorage settings)
        {
            _account = CloudStorageAccount.Parse(settings.ConnectionString);
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

            if (!await file.ExistsAsync())
                return null;

            var data = new byte[file.StreamWriteSizeInBytes];
            await file.DownloadToByteArrayAsync(data, 0);

            return data;
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
}
