using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Homespirations.Core.Helpers;
using NUlid;

public class User
{
    [Key]
    [JsonConverter(typeof(UlidJsonConverter))]
    public string Id { get; set; } = Ulid.NewUlid().ToString(); // Stored as string

    public string Email { get; set; } = "";
    public string Hash { get; set; } = "";
}
