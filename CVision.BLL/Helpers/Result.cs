namespace CVision.BLL.Helpers;

public class Result<T>
{
    private Result(T value)
    {
        Value = value;
        Error = null;
    }

    private Result(string error)
    {
        Value = default;
        Error = error;
    }

    public T? Value { get; }

    public string? Error { get; }

    public bool IsSuccess => Error == null;

    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(string error) => new(error);

    public static Result<T> Ok(T value) => new(value);
}
