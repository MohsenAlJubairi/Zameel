namespace Zameel.Application.Common;

public class ServiceResult<T>
{
    public bool isSuccess { get; set; }
    public string message { get; set; } = string.Empty;
    public T? data { get; set; }
    public List<string> errors { get; set; } = new List<string>();

    public static ServiceResult<T> Success(T data, string message = "")
    {
        return new ServiceResult<T>
        {
            isSuccess = true,
            data = data,
            message = message
        };
    }

    public static ServiceResult<T> Fail(string message, List<string>? errors = null)
    {
        return new ServiceResult<T>
        {
            isSuccess = false,
            message = message,
            errors = errors ?? new List<string>()
        };
    }
}

public class ServiceResult : ServiceResult<bool>
{
    public static ServiceResult Success(string message = "")
    {
        return new ServiceResult
        {
            isSuccess = true,
            data = true,
            message = message
        };
    }

    public static new ServiceResult Fail(string message, List<string>? errors = null)
    {
        return new ServiceResult
        {
            isSuccess = false,
            data = false,
            message = message,
            errors = errors ?? new List<string>()
        };
    }
}
