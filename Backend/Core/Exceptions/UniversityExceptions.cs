
namespace Core.Exceptions;

public class UniversityNotFoundException : Exception, ICustomException
{
    public UniversityNotFoundException(string message) : base(message) { }
    public UniversityNotFoundException() { }
    public UniversityNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}


public class UniversityAlreadyExists : Exception, ICustomException
{
    public UniversityAlreadyExists(string message) : base(message) { }
    public UniversityAlreadyExists() { }
    public UniversityAlreadyExists(string? message, Exception? innerException) : base(message, innerException) { }
}


public class UserUniversityNotFoundException : Exception, ICustomException
{
    public UserUniversityNotFoundException(string message) : base(message) { }
    public UserUniversityNotFoundException() { }
    public UserUniversityNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}
