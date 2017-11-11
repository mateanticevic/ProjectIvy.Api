using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.Text;
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

        public async Task<byte[]> GetFile(string fileName)
        {
            var parts = fileName.Split('/');

            var share = _client.GetShareReference(parts[0]);
            var directory = share.GetRootDirectoryReference();

            var file = directory.GetFileReference(parts[1]);

            byte[] data = new byte[file.StreamWriteSizeInBytes];
            await file.DownloadToByteArrayAsync(data, 0);

            return data;
        }
    }
}
