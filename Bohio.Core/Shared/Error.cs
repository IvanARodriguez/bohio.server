namespace Bohio.Core.Results;

public class Error(string code, string message)
{
    public string Code { get; } = code;
    public string Message { get; } = message;

    public static Error None => new(string.Empty, string.Empty);
}
