using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Homespirations.Core.Entities;
using Homespirations.Core.Results;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Homespirations.Core.Interfaces
{
    public interface IMediaServices
    {
        Task<Result<List<MediaDto>>> UploadMediaAsync(Ulid homeSpaceId, IFormFileCollection files);
    }
}