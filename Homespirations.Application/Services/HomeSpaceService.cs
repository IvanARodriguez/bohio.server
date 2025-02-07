using Homespirations.Core.Interfaces;
using Homespirations.Core.Entities;
using Homespirations.Core.Results;
using NUlid;
using Application.Common.Errors;

namespace Homespirations.Application.Services;

public class HomeSpaceService(IUnitOfWork unitOfWork)
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;

  public async Task<Result<IEnumerable<HomeSpace>>> GetAllHomeSpacesAsync()
  {
    var homeSpaces = await _unitOfWork.HomeSpaces.GetAllAsync();
    return Result<IEnumerable<HomeSpace>>.Success(homeSpaces);
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
    await _unitOfWork.HomeSpaces.AddAsync(homeSpace);
    await _unitOfWork.SaveChangesAsync();
    return Result.Success();
  }

  public async Task<Result> UpdateHomeSpaceAsync(HomeSpace homeSpace)
  {
    var existingHomeSpace = await _unitOfWork.HomeSpaces.GetByIdAsync(homeSpace.Id);
    if (existingHomeSpace is null)
      return Result.Failure(Errors.HomeSpace.NotFound);

    await _unitOfWork.HomeSpaces.UpdateAsync(homeSpace);
    await _unitOfWork.SaveChangesAsync();
    return Result.Success();
  }

  public async Task<Result> DeleteHomeSpaceAsync(Ulid id)
  {
    var existingHomeSpace = await _unitOfWork.HomeSpaces.GetByIdAsync(id);
    if (existingHomeSpace is null)
      return Result.Failure(Errors.HomeSpace.NotFound);

    await _unitOfWork.HomeSpaces.DeleteAsync(id);
    await _unitOfWork.SaveChangesAsync();
    return Result.Success();
  }
}
