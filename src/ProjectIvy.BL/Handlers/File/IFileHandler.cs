using ProjectIvy.Model.Binding.File;
using ProjectIvy.Model.View.File;
using System.Threading.Tasks;

namespace ProjectIvy.BL.Handlers.File
{
    public interface IFileHandler
    {
        System.Threading.Tasks.Task DeleteFile(string id);

        Task<FileWithData> GetFile(string id);

        Task<string> UploadFile(FileBinding file);
    }
}