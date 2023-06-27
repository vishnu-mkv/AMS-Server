namespace AMS.Interfaces
{
    public interface IFileManager
    {
            void DeleteFile(string filePath);
            public string Save(string directory, IFormFile file);
            public string GetFullUrl(string filePath);
    }
}
