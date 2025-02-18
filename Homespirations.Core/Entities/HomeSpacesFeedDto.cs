using System.Text.Json.Serialization;
using Homespirations.Core.Helpers;
using NUlid;

namespace Homespirations.Core.Entities
{
    public class HomeSpacesFeedDto
    {
        [JsonConverter(typeof(UlidJsonConverter))]
        public Ulid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<MediaDto>? MediaItems { get; set; } = [];
    }
}