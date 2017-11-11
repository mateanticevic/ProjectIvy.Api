using System.Threading.Tasks;

namespace ProjectIvy.BL.Handlers.File
{
    public interface IFileHandler
    {
        Task<byte[]> GetFile(string id);
    }
}