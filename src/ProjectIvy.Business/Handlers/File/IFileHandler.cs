using ProjectIvy.Model.Binding.File;
using ProjectIvy.Model.View.File;
using System.Threading.Tasks;

namespace ProjectIvy.Business.Handlers.File;

public interface IFileHandler
{
    Task DeleteFile(string id);

    Task<FileWithData> GetFile(string id);

    Task<string> UploadFile(FileBinding file);

    Task<Model.Database.Main.Storage.File> UploadFileInternal(FileBinding file);
}
