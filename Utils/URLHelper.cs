namespace AMS.Utils
{
    public class URLHelper
    {
        public static string GetFileURL(IHttpContextAccessor _httpContextAccessor, string filePath)
        {
            if (filePath.StartsWith("http://") || filePath.StartsWith("https://"))
            {
                return filePath;
            }

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/{filePath.Replace("\\", "/")}";
        }
    }

}
