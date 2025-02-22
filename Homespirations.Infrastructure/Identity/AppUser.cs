using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using NUlid;

public class AppUser : IdentityUser
{
    [JsonIgnore]
    public string UserId { get; set; } = ""; // Foreign key stored as string

    [NotMapped] // Used in the application but not stored in DB
    public Ulid UlidUserId
    {
        get => Ulid.Parse(UserId);
        set => UserId = value.ToString();
    }

    public required User User { get; set; }
}
