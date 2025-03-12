using System.Text.Json.Serialization;
using Bohio.Core.Helpers;
using NUlid;

namespace Bohio.Core.Entities
{
    public class HomeSpacesFeed
    {
        [JsonConverter(typeof(UlidJsonConverter))]
        public Ulid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<MediaRequest>? MediaItems { get; set; } = [];
    }
}