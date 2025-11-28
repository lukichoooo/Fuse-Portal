
namespace Core.Exceptions;

public class UniversityNotFoundException : Exception, ICustomException
{
    public UniversityNotFoundException(string message) : base(message) { }
    public UniversityNotFoundException() { }
    public UniversityNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}

