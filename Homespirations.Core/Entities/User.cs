using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Homespirations.Core.Helpers;
using NUlid;


namespace Homespirations.Core.Entities
{
    public class User
    {
        [NotMapped]
        [JsonConverter(typeof(UlidJsonConverter))]
        public Ulid Id { get; set; }
        public string Email { get; set; } = "";
        public string Hash { get; set; } = "";
    }
}