using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bohio.Core.Interfaces
{
    public interface ITokenService
    {
        Task<string?> GetUserIdFromTokenAsync(string accessToken);
    }
}