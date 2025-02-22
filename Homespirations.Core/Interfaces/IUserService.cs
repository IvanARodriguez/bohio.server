using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Homespirations.Core.DTOs;
using Homespirations.Core.Results;

namespace Homespirations.Core.Interfaces;
public interface IUserService
{
    Task<(bool Success, List<Error>? Errors, User? User)> CreateUserAsync(RegisterRequest request);
}
