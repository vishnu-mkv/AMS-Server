using System.Text.Json;
using System.Text.Json.Serialization;

namespace AMS.Utils
{
    public class PhotoUrlConverter : JsonConverter<string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(string);
        }

        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (string.IsNullOrEmpty(value))
            {
                writer.WriteNullValue();
                return;
            }
            
                // Use the URLHelper to get the full URL and set it as the value of the photo URL member
            string url = URLHelper.GetFileURL(_httpContextAccessor, value);
            writer.WriteStringValue(url);
        }
    }

}
