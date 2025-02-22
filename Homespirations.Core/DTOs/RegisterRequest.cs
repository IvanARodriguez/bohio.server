using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homespirations.Core.DTOs;

public class RegisterRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
