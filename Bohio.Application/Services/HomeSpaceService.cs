using Bohio.Core.Interfaces;
using Bohio.Core.Entities;
using Bohio.Core.Results;
using NUlid;
using Application.Common.Errors;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Bohio.Application.Services;

public class HomeSpaceService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<HomeSpace> validator, ILogger<HomeSpaceService> logger)
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  private readonly IMapper _mapper = mapper;
  private readonly IValidator<HomeSpace> _validator = validator;
  private readonly ILogger<HomeSpaceService> _logger = logger;

  public async Task<Result<IEnumerable<HomeSpacesFeed>>> GetAllHomeSpacesAsync()
  {
    var homeSpaces = await _unitOfWork.HomeSpaceAndMedia.GetHomeSpaceAndImagesAsync();
    var mappedHomeSpaces = _mapper.Map<IEnumerable<HomeSpacesFeed>>(homeSpaces);
    return Result<IEnumerable<HomeSpacesFeed>>.Success(mappedHomeSpaces ?? []);
  }

  public async Task<Result<HomeSpace>> GetHomeSpaceByIdAsync(Ulid id)
  {
    var homeSpace = await _unitOfWork.HomeSpaces.GetByIdAsync(id);
    return homeSpace is not null
        ? Result<HomeSpace>.Success(homeSpace)
        : Result<HomeSpace>.Failure(Errors.HomeSpace.NotFound);
  }

  public async Task<Result> AddHomeSpaceAsync(HomeSpace homeSpace)
  {
    if (homeSpace == null)
      return Result.Failure(new Error("Validation", "HomeSpace cannot be null."));

    var validationResult = await _validator.ValidateAsync(homeSpace);

    if (!validationResult.IsValid)
    {
      var errors = validationResult.Errors
          .Select(e => new Error(e.PropertyName, e.ErrorMessage))
          .ToList();

      return Result.Failure(errors);
    }

    try
    {
      await _unitOfWork.HomeSpaces.AddAsync(homeSpace);
      await _unitOfWork.SaveChangesAsync();
      return Result.Success();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error adding HomeSpace");

      return Result.Failure(new Error("Database", "An error occurred while saving the HomeSpace."));
    }
  }

  public async Task<Result> UpdateHomeSpaceAsync(HomeSpace homeSpace)
  {
    if (homeSpace == null)
      return Result.Failure(new Error("Validation", "HomeSpace cannot be null."));

    if (string.IsNullOrWhiteSpace(homeSpace.Id.ToString()))
      return Result.Failure(new Error("Validation", "HomeSpace ID is required."));

    var existingHomeSpace = await _unitOfWork.HomeSpaces.GetByIdAsync(homeSpace.Id);
    if (existingHomeSpace is null)
      return Result.Failure(new Error("NotFound", "HomeSpace not found."));

    var validationResult = await _validator.ValidateAsync(homeSpace);
    if (!validationResult.IsValid)
    {
      var errors = validationResult.Errors
          .Select(e => new Error(e.PropertyName, e.ErrorMessage))
          .ToList();

      return Result.Failure(errors);
    }

    try
    {
      _mapper.Map(homeSpace, existingHomeSpace);
      await _unitOfWork.SaveChangesAsync();
      return Result.Success();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating HomeSpace");

      return Result.Failure(new Error("Database", "An error occurred while updating the HomeSpace."));
    }
  }



  public async Task<Result> DeleteHomeSpaceAsync(Ulid id)
  {
    if (id == Ulid.Empty)
      return Result.Failure(new Error("Validation", "HomeSpace ID is required."));

    var existingHomeSpace = await _unitOfWork.HomeSpaces.GetByIdAsync(id);
    if (existingHomeSpace is null)
      return Result.Failure(new Error("NotFound", "HomeSpace not found."));

    try
    {
      await _unitOfWork.HomeSpaces.DeleteAsync(id);
      await _unitOfWork.SaveChangesAsync();
      return Result.Success();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error deleting HomeSpace");

      return Result.Failure(new Error("Database", "An error occurred while deleting the HomeSpace."));
    }
  }

  // public async Task<Result> AddMediaToHomeSpaceAsync(Ulid homespaceId, FormFileCollection files)
  // {
  //   if (homespaceId == Ulid.Empty)
  //     return Result.Failure(new Error("Validation", "HomeSpace ID is required."));

  //   var existingHomeSpace = await _unitOfWork.HomeSpaces.GetByIdAsync(homespaceId);
  //   if (existingHomeSpace is null)
  //     return Result.Failure(new Error("NotFound", "HomeSpace not found."));

  //   try
  //   {
  //     foreach (var file in files){

  //     }
  //   }
  // }

}
