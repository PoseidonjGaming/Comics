using System.Text.Json;
using System.Text.Json.Serialization;

namespace ComicsJDownloaderApi.Helpers
{
    internal class EnumJsonConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? raw= reader.GetString();

            if (raw is null)
                return default;

            string normalized = raw.Replace("_", string.Empty).ToUpperInvariant();

            foreach (var name in Enum.GetNames(typeof(T)))
            {
                string enumNormalized = name.Replace("_", string.Empty).ToUpperInvariant();
                if (enumNormalized == normalized)
                {
                    return Enum.Parse<T>(name);
                }
            }
            return default;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
