using AutoMapper;
using Bohio.Core.Entities;
using Bohio.Core.Interfaces;
using Bohio.Core.Results;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Bohio.Application.Services
{
  public class MediaServices(
      IMapper mapper,
      IImageOptimizer imageOptimizer,
      ICloudStorage cloudStorage,
      IUnitOfWork unitOfWork) : IMediaServices
  {
    private readonly IImageOptimizer _imageOptimizer = imageOptimizer;
    private readonly ICloudStorage _cloudStorage = cloudStorage;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    private readonly IMapper _mapper = mapper;

    public async Task<Result<List<MediaRequest>>> UploadMediaAsync(Ulid homeSpaceId, IFormFileCollection files)
    {
      var homeSpace = await _unitOfWork.HomeSpaces.GetByIdAsync(homeSpaceId);
      if (homeSpace == null)
      {
        return Result<List<MediaRequest>>.Failure(new Error("NotFound", "HomeSpace not found."));
      }

      var uploadedMedia = new List<MediaRequest>();

      foreach (var file in files)
      {
        if (!file.ContentType.StartsWith("image/"))
        {
          continue;
        }

        using var stream = file.OpenReadStream();
        byte[] optimizedImage = await _imageOptimizer.OptimizeAsync(stream, 1280);

        string fileName = $"homespace_media_{Ulid.NewUlid()}.webp";
        string cloudflareUrl = await _cloudStorage.UploadAsync(optimizedImage, fileName, "image/webp");

        var media = new Media
        {
          HomeSpaceId = homeSpaceId,
          Url = cloudflareUrl,
          MediaType = MediaType.Image
        };

        await _unitOfWork.Media.AddAsync(media);
        uploadedMedia.Add(_mapper.Map<MediaRequest>(media));
      }

      // 🔥 Commit the changes to the database
      await _unitOfWork.SaveChangesAsync();

      return Result<List<MediaRequest>>.Success(uploadedMedia);
    }

  }
}
