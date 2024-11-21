using TomNam.Models;
namespace TomNam.Interfaces{
    public interface IUploadService
    {
        public string Upload(IFormFile file, string UploadPath);
    }
}