namespace Core.Exceptions;


public class MessageNotFoundException : Exception, ICustomException
{
    public MessageNotFoundException(string message) : base(message) { }
    public MessageNotFoundException() { }
    public MessageNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}


public class ChatNotFoundException : Exception, ICustomException
{
    public ChatNotFoundException(string message) : base(message) { }
    public ChatNotFoundException() { }
    public ChatNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}


public class FileTooLargeException : Exception, ICustomException
{
    public FileTooLargeException(string message) : base(message) { }
    public FileTooLargeException() { }
    public FileTooLargeException(string? message, Exception? innerException) : base(message, innerException) { }
}
