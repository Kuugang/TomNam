using TomNam.Models;
namespace TomNam.Interfaces{
    public interface IFileUploadService
    {
        public string Upload(IFormFile file, string UploadPath);
    }
}