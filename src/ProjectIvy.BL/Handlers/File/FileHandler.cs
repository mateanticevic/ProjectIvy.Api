using ProjectIvy.DL.Services.AzureStorage;
using System.Threading.Tasks;

namespace ProjectIvy.BL.Handlers.File
{
    public class FileHandler : Handler<FileHandler>, IFileHandler
    {
        private readonly IAzureStorageHelper _azureStorageHelper;

        public FileHandler(IHandlerContext<FileHandler> context, IAzureStorageHelper azureStorageHelper) : base(context)
        {
            _azureStorageHelper = azureStorageHelper;
        }

        public async Task<byte[]> GetFile(string id)
        {
            return await _azureStorageHelper.GetFile($"images/{id}.jpg");
        }
    }
}
