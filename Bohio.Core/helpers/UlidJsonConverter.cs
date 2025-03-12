using System.Text.Json;
using System.Text.Json.Serialization;
using NUlid;

namespace Bohio.Core.Helpers;

public class UlidJsonConverter : JsonConverter<Ulid>
{
  public override Ulid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    var stringValue = reader.GetString();
    return Ulid.TryParse(stringValue, out var ulid) ? ulid : Ulid.NewUlid();
  }

  public override void Write(Utf8JsonWriter writer, Ulid value, JsonSerializerOptions options)
  {
    writer.WriteStringValue(value.ToString());
  }
}
