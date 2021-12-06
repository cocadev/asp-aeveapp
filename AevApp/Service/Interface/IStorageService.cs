using System.IO;
using System.Threading.Tasks;

namespace AevApp.Service.Interface
{
    public interface IStorageService
    {
        Task UploadImageAsync(Stream fileStream, string fileName, string accountName = null, string accountKey = null, string blobContainer = null);
    }
}