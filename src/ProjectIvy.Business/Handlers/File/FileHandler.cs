using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Services.AzureStorage;
using ProjectIvy.Model.Binding.File;
using ProjectIvy.Model.Constants.Database;
using ProjectIvy.Model.View.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectIvy.Business.Handlers.File
{
    public class FileHandler : Handler<FileHandler>, IFileHandler
    {
        private readonly IAzureStorageHelper _azureStorageHelper;

        public FileHandler(IHandlerContext<FileHandler> context, IAzureStorageHelper azureStorageHelper) : base(context)
        {
            _azureStorageHelper = azureStorageHelper;
        }

        public async System.Threading.Tasks.Task DeleteFile(string id)
        {
            using (var context = GetMainContext())
            {
                var file = context.Files.Include(x => x.FileType)
                                        .SingleOrDefault(x => x.ValueId == id);

                if (file.UserId != User.Id)
                    throw new UnauthorizedException();

                await _azureStorageHelper.DeleteFile(file.Uri);
            }
        }

        public async Task<FileWithData> GetFile(string id)
        {
            using (var context = GetMainContext())
            {
                var file = context.Files.Include(x => x.FileType)
                                        .SingleOrDefault(x => x.ValueId == id);

                var data = await _azureStorageHelper.GetFile(file.Uri);

                if (data == null)
                    throw new ResourceNotFoundException();

                return new FileWithData(file) { Data = data };
            }
        }

        public async Task<string> UploadFile(FileBinding file)
        {
            string fileName = Guid.NewGuid().ToString().Replace("-", string.Empty);

            using (var context = GetMainContext())
            {
                var fileType = context.FileTypes.SingleOrDefault(x => x.MimeType == file.MimeType);

                string fullPath = $"{GetFolder((StorageFileType)fileType.Id)}/{fileName}.{fileType.Extension}";

                await _azureStorageHelper.UploadFile(fullPath, file.Data);

                var fileEntity = new Model.Database.Main.Storage.File()
                {
                    Created = DateTime.Now,
                    FileTypeId = fileType.Id,
                    ProviderId = (int)StorageProvider.AzureStorage,
                    SizeInBytes = file.Data.Length,
                    Uri = fullPath,
                    UserId = User.Id,
                    ValueId = fileName
                };
                context.Files.Add(fileEntity);
                await context.SaveChangesAsync();
            }

            return fileName;
        }

        private string GetFolder(StorageFileType fileType)
        {
            // TODO: Rewrite
            var documents = new List<StorageFileType>() { StorageFileType.Pdf, StorageFileType.Msg };
            var images = new List<StorageFileType>() { StorageFileType.Jpg, StorageFileType.Png };
            var audio = new List<StorageFileType>() { StorageFileType.Mp3 };

            if (images.Contains(fileType))
                return "images";

            if (documents.Contains(fileType))
                return "documents";

            if (audio.Contains(fileType))
                return "audio";

            return "other";
        }
    }
}
