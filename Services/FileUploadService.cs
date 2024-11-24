using TomNam.Interfaces;

namespace TomNam.Services{

    public class FileUploadService : IFileUploadService{
        public string Upload(IFormFile file, string UploadPath){
            // extension
            List<string> ValidExtensions = new List<string>() {".jpg", ".jpeg", ".png", ".gif", ".pdf"};
            string extension = Path.GetExtension(file.FileName);
            if (!ValidExtensions.Contains(extension)){
                return $"Invalid file extension({string.Join(", ", ValidExtensions)})";
            }

            // file size
            long Size = file.Length;
            if (Size > 10 * 1024 * 1024){
                return "Maximum file size is 10MB";
            }

            // name change 
            string fileName = Guid.NewGuid().ToString() + extension;
            String RelativePath = Path.Combine("Uploads", UploadPath);
            String FullPath = Path.Combine(Directory.GetCurrentDirectory(), RelativePath, fileName)
              .Replace("\\","/");

            Console.WriteLine(FullPath);
            using FileStream stream = new FileStream(FullPath, FileMode.Create);

            file.CopyTo(stream);
            return Path.Combine(RelativePath, fileName)
              .Replace("\\", "/");
        }
    }
}
