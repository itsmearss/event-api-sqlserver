
namespace TestProjectAnnur.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public void DeleteFile(string fileNameWithExtension)
        {
            // Check param fileNameWithExtension
            if(string.IsNullOrEmpty(fileNameWithExtension))
            {
                throw new ArgumentException(nameof(fileNameWithExtension));
            }

            // Get path
            var contentPath = _environment.ContentRootPath;

            // Image path
            var path = Path.Combine(contentPath, $"Uploads", fileNameWithExtension);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Invalid file path");
            }
            File.Delete(path);
        }

        public async Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions)
        {
            if(imageFile == null)
            {
                throw new ArgumentNullException(nameof(imageFile));
            }

            var contentPath = _environment.ContentRootPath;
            var path = Path.Combine(contentPath, "Uploads");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Check the allowed file extensions
            var ext = Path.GetExtension(imageFile.FileName);
            if(!allowedFileExtensions.Contains(ext))
            {
                throw new ArgumentException($"Only {string.Join(",", allowedFileExtensions)} are allowed");
            }

            // generate a unique filename
            var fileName = $"{Guid.NewGuid().ToString()}{ext}";
            var fileNameWithPath = Path.Combine(path, fileName);
            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await imageFile.CopyToAsync(stream);
            return fileName;
        }
    }
}
