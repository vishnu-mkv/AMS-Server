using AMS.Interfaces;
using AMS.Utils;

namespace AMS.Managers
{
    public class LocalFileManager : IFileManager
    {
        private string wwwrootPath;

        public IHttpContextAccessor _httpContextAccessor { get; }

        public LocalFileManager(IWebHostEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            wwwrootPath = hostingEnvironment.WebRootPath;
            _httpContextAccessor = httpContextAccessor;
        }

        public string Save(string directory, IFormFile file)
        {
            string photoName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string filePath = Path.Combine("media", directory, photoName);

            if (!Directory.Exists("wwwroot"))
            {
                // Create wwwroot directory
                Directory.CreateDirectory("wwwroot");
                wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            string fullPath = Path.Combine(wwwrootPath, filePath);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return filePath;
        }

        public void DeleteFile(string filePath)
        {
            string fullPath = Path.Combine(wwwrootPath, filePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public string GetFullUrl(string filePath)
        {
            return URLHelper.GetFileURL(_httpContextAccessor, filePath);
        }


    }
}