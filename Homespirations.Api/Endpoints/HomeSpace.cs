using Application.Common.Errors;
using Homespirations.Application.Services;
using Homespirations.Core.Entities;
using Homespirations.Core.Results;
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
  }

  private static async Task<Results<Ok<IEnumerable<HomeSpace>>, NotFound<Error>>> GetAll(
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
    return TypedResults.NotFound<Error>(Errors.HomeSpace.NotFound);
  }

  private static async Task<Results<Created, Conflict<Error>, BadRequest<Error>>> Create(
     [FromBody] HomeSpace homeSpace,
     [FromServices] HomeSpaceService service)
  {
    if (homeSpace is not null && homeSpace.Id.ToString() != "")
    {
      var existing = await service.GetHomeSpaceByIdAsync(homeSpace.Id);
      if (existing.IsSuccess) // Check if the home space already exists
        return TypedResults.Conflict(Errors.HomeSpace.AlreadyExists);
    }
    if (homeSpace == null)
    {
      return TypedResults.BadRequest(Errors.HomeSpace.InvalidData);
    }

    var result = await service.AddHomeSpaceAsync(homeSpace);
    if (result.IsSuccess)
    {
      string location = $"/api/homespace/{homeSpace.Id}";
      return TypedResults.Created(location);
    }

    return TypedResults.BadRequest(Errors.HomeSpace.InvalidData);
  }

  private static async Task<Results<NoContent, NotFound<Error>, BadRequest<Error>>> Update(
     string id,
     [FromBody] HomeSpace homeSpace,
     [FromServices] HomeSpaceService service)
  {
    var result = await service.UpdateHomeSpaceAsync(homeSpace);
    if (!result.IsSuccess)
      return TypedResults.BadRequest(Errors.HomeSpace.InvalidData);

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

}
