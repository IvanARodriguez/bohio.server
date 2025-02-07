using Application.Common.Errors;
using Homespirations.Application.Services;
using Homespirations.Core.Entities;
using Homespirations.Core.Results;
using Microsoft.AspNetCore.Http.HttpResults;
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

  private static async Task<Results<Ok<IEnumerable<HomeSpace>>, NotFound<Error>>> GetAll(HomeSpaceService service)
  {
    var result = await service.GetAllHomeSpacesAsync();

    if (result.IsSuccess)
    {
      return TypedResults.Ok(result.Value); // Use the `Value` from the Result if success
    }

    return TypedResults.NotFound(Errors.HomeSpace.NotFound); // Or return an error if it failed
  }

  private static async Task<Results<Ok<HomeSpace>, NotFound<Error>>> GetById(Ulid id, HomeSpaceService service)
  {
    var result = await service.GetHomeSpaceByIdAsync(id);
    if (result.IsSuccess)
    {
      return TypedResults.Ok(result.Value); // Use the `Value` from the Result if success
    }
    return TypedResults.NotFound<Error>(Errors.HomeSpace.NotFound);
  }

  private static async Task<Results<CreatedAtRoute<HomeSpace>, Conflict<Error>, BadRequest<Error>>> Create(
      HomeSpace homeSpace, HomeSpaceService service)
  {
    if (homeSpace == null)
      return TypedResults.BadRequest(Errors.HomeSpace.InvalidData);

    var existing = await service.GetHomeSpaceByIdAsync(homeSpace.Id);
    if (existing is not null)
      return TypedResults.Conflict(Errors.HomeSpace.AlreadyExists);

    await service.AddHomeSpaceAsync(homeSpace);
    return TypedResults.CreatedAtRoute(homeSpace, nameof(GetById), new { id = homeSpace.Id });
  }

  private static async Task<Results<NoContent, NotFound<Error>, BadRequest<Error>>> Update(
      Ulid id, HomeSpace homeSpace, HomeSpaceService service)
  {
    if (id != homeSpace.Id)
      return TypedResults.BadRequest(Errors.HomeSpace.InvalidData);

    var existing = await service.GetHomeSpaceByIdAsync(id);
    if (existing is null)
      return TypedResults.NotFound(Errors.HomeSpace.NotFound);

    await service.UpdateHomeSpaceAsync(homeSpace);
    return TypedResults.NoContent();
  }

  private static async Task<Results<NoContent, NotFound<Error>>> Delete(Ulid id, HomeSpaceService service)
  {
    var existing = await service.GetHomeSpaceByIdAsync(id);
    if (existing is null)
      return TypedResults.NotFound(Errors.HomeSpace.NotFound);

    await service.DeleteHomeSpaceAsync(id);
    return TypedResults.NoContent();
  }
}
