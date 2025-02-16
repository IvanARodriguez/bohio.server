namespace Homespirations.Core.Results;

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
  public static Result Failure(List<Error> errors) => new(false, errors);
  public static Result Failure(Error error) => new(false, new List<Error> { error });
}


public class Result<T> : Result
{
  public T? Value { get; }

  private Result(T value) : base(true) => Value = value;
  private Result(List<Error> errors) : base(false, errors) { }

  public static Result<T> Success(T value) => new(value);
  public static new Result<T> Failure(List<Error> errors) => new(errors);
  public static new Result<T> Failure(Error error) => new([error]);
}
