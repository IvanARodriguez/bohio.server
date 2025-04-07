using Bohio.Core.Entities;
using Bohio.Core.Results;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Bohio.Core.Interfaces
{
  public interface IMediaServices
  {
    Task<Result<List<MediaRequest>>> UploadMediaAsync(Ulid homeSpaceId, IFormFileCollection files);
  }
}
