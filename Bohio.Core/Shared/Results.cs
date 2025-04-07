namespace Bohio.Core.Results;

public class Result
{
  public bool IsSuccess { get; }
  public bool IsFailure => !IsSuccess;
  public List<Error> Errors { get; }

  protected Result(bool isSuccess, List<Error>? errors = null)
  {
    IsSuccess = isSuccess;
    Errors = errors ?? [];
  }

  public static Result Success() => new(true);
  public static Result Success(string msg) => new(true);

  public static Result Failure(List<Error> errors) => new(false, errors);
  public static Result Failure(Error error) => new(false, new List<Error> { error });
}


public class Result<T> : Result
{
  public T? Value { get; }

  // Constructor for Result<T> using the base Result constructor
  public Result(bool isSuccess, T value, List<Error>? errors = null)
      : base(isSuccess, errors)
  {
    Value = value;
  }

  // Static method for success
  public static Result<T> Success(T value) => new(true, value);

  // Static methods for failure
  public static new Result<T> Failure(List<Error> errors) => new(false, default!, errors);
  public static new Result<T> Failure(Error error) => new(false, default!, [error]);
}
