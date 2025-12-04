namespace Core.Exceptions;

public class LMStudioApiException : Exception, ICustomException
{
    public int? StatusCode { get; init; }

    public LMStudioApiException(string message) : base(message) { }
    public LMStudioApiException() { }
    public LMStudioApiException(string? message, Exception? innerException) : base(message, innerException) { }
    public LMStudioApiException(string? message, int? statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}
