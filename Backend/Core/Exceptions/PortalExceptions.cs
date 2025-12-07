namespace Core.Exceptions;


public class SubjectNotFoundException : Exception, ICustomException
{
    public SubjectNotFoundException(string message) : base(message) { }
    public SubjectNotFoundException() { }
    public SubjectNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}
