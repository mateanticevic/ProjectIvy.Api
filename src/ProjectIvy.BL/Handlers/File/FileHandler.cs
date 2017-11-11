using Microsoft.EntityFrameworkCore;
using ProjectIvy.DL.Services.AzureStorage;
using ProjectIvy.Model.View.File;
using System.Linq;
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

        public async Task<FileWithData> GetFile(string id)
        {
            using (var context = GetMainContext())
            {
                var file = context.Files.Include(x => x.FileType)
                                        .SingleOrDefault(x => x.ValueId == id);

                var data = await _azureStorageHelper.GetFile(file.Uri);

                return new FileWithData(file) { Data = data };
            }
        }
    }
}
