using System.Text.Json.Serialization;
using Homespirations.Core.Helpers;
using NUlid;

namespace Homespirations.Core.Entities
{
    public class MediaDto
    {
        [JsonConverter(typeof(UlidJsonConverter))]
        public Ulid Id { get; set; } = Ulid.NewUlid();
        public string Url { get; set; } = string.Empty;
    }
}