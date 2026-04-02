namespace CVision.BLL.Helpers;

public class MyResult<T>
{
    private MyResult(T value)
    {
        Value = value;
        Error = null;
    }

    private MyResult(string error)
    {
        Value = default;
        Error = error;
    }

    public T? Value { get; }

    public string? Error { get; }

    public bool IsSuccess => Error == null;

    public static implicit operator MyResult<T>(T value) => new(value);

    public static implicit operator MyResult<T>(string error) => new(error);
}