using Application.Common.Errors;
using Homespirations.Application.Services;
using Homespirations.Core.Entities;
using Homespirations.Core.Results;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NUlid;

namespace Homespirations.Api.Endpoints;

public static class HomeSpaceEndpoints
{
  public static void MapHomeSpaceEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/homespace").WithTags("HomeSpace");

    group.MapGet("/", GetAll);
    group.MapGet("/{id}", GetById);
    group.MapPost("/", Create);
    group.MapPut("/{id}", Update);
    group.MapDelete("/{id}", Delete);
    group.MapPost("/{id}/upload", AddMedia).DisableAntiforgery();
  }

  private static async Task<Results<Ok<IEnumerable<HomeSpacesFeedDto>>, NotFound<Error>>> GetAll(
    [FromServices] HomeSpaceService service)
  {
    var result = await service.GetAllHomeSpacesAsync();

    if (result.IsSuccess && result.Value is not null)
    {
      return TypedResults.Ok(result.Value);
    }

    return TypedResults.NotFound(Errors.HomeSpace.NotFound);
  }

  private static async Task<Results<Ok<HomeSpace>, NotFound<Error>>> GetById(
      Ulid id,
      [FromServices] HomeSpaceService service)
  {
    var result = await service.GetHomeSpaceByIdAsync(id);
    if (result.IsSuccess)
    {
      return TypedResults.Ok(result.Value);
    }
    return TypedResults.NotFound(Errors.HomeSpace.NotFound);
  }

  private static async Task<Results<Created, Conflict<Error>, BadRequest<List<Error>>>> Create(
    [FromBody] HomeSpace homeSpace,
    [FromServices] HomeSpaceService service)
  {
    if (homeSpace is null)
    {
      return TypedResults.BadRequest(new List<Error> { Errors.HomeSpace.InvalidData });
    }

    // Check if HomeSpace already exists (avoid duplicates)
    if (homeSpace.Id != Ulid.Empty)
    {
      var existing = await service.GetHomeSpaceByIdAsync(homeSpace.Id);
      if (existing.IsSuccess)
      {
        return TypedResults.Conflict(Errors.HomeSpace.AlreadyExists);
      }
    }

    // Add new HomeSpace and handle validation errors properly
    var result = await service.AddHomeSpaceAsync(homeSpace);

    if (!result.IsSuccess)
    {
      return TypedResults.BadRequest(result.Errors); // Return all validation errors
    }

    string location = $"/api/homespace/{homeSpace.Id}";
    return TypedResults.Created(location);
  }

  private static async Task<Results<NoContent, NotFound<Error>, BadRequest<List<Error>>>> Update(
     string id,
     [FromBody] HomeSpace homeSpace,
     [FromServices] HomeSpaceService service)
  {
    if (homeSpace is null)
    {
      return TypedResults.BadRequest(new List<Error> { Errors.HomeSpace.InvalidData });
    }

    if (homeSpace.Id == Ulid.Empty)
    {
      return TypedResults.BadRequest(new List<Error> { Errors.HomeSpace.InvalidData });
    }

    if (id != homeSpace.Id.ToString())
    {
      return TypedResults.BadRequest(new List<Error> { Errors.HomeSpace.InvalidData });
    }

    var result = await service.UpdateHomeSpaceAsync(homeSpace);

    if (!result.IsSuccess)
      return TypedResults.BadRequest(result.Errors);

    return TypedResults.NoContent();
  }

  private static async Task<Results<NoContent, NotFound<Error>>> Delete(
      Ulid id,
      [FromServices] HomeSpaceService service)
  {
    var existing = await service.GetHomeSpaceByIdAsync(id);
    if (existing is null)
      return TypedResults.NotFound(Errors.HomeSpace.NotFound);

    await service.DeleteHomeSpaceAsync(id);
    return TypedResults.NoContent();
  }

  [IgnoreAntiforgeryToken]
  private static async Task<Results<Ok<List<MediaDto>>, NotFound<Error>, BadRequest<List<Error>>>> AddMedia(
      string id,
      [FromServices] HomeSpaceService service,
      [FromServices] MediaServices mediaServices,
      [FromForm] IFormFileCollection files)
  {
    // Check for invalid or missing ID
    if (id is null || string.IsNullOrWhiteSpace(id) || !Ulid.TryParse(id, out var ulid))
    {
      return TypedResults.BadRequest(new List<Error> { Errors.Media.InvalidId });
    }

    // Upload media files
    var result = await mediaServices.UploadMediaAsync(ulid, files);

    // Check for any upload issues and return success or failure
    if (!result.IsSuccess)
    {
      return TypedResults.BadRequest(result.Errors);
    }

    // Return the successfully uploaded media
    return TypedResults.Ok<List<MediaDto>>(result.Value);
  }


}
