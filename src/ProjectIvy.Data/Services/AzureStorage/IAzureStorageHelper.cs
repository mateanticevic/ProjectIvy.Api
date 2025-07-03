using System.Threading.Tasks;

namespace ProjectIvy.Data.Services.AzureStorage;

public interface IAzureStorageHelper
{
    Task DeleteFile(string fileName);

    Task<byte[]> GetFile(string fileName);

    Task UploadFile(string fileName, byte[] fileData);
}