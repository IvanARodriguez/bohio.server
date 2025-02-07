namespace Homespirations.Core.Results;

public class Result
{
  public bool IsSuccess { get; }
  public Error? Error { get; }

  protected Result(bool isSuccess, Error? error = null)
  {
    IsSuccess = isSuccess;
    Error = error;
  }

  public static Result Success() => new(true);
  public static Result Failure(Error error) => new(false, error);
}

public class Result<T> : Result
{
  public T? Value { get; }

  private Result(T value) : base(true) => Value = value;
  private Result(Error error) : base(false, error) { }

  public static Result<T> Success(T value) => new(value);
  public static Result<T> Failure(Error error) => new(error);
}
