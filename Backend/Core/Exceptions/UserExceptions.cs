namespace Core.Exceptions;


public class UserWrongCredentialsException : Exception, ICustomException
{
    public UserWrongCredentialsException(string message) : base(message) { }
    public UserWrongCredentialsException() { }
    public UserWrongCredentialsException(string? message, Exception? innerException) : base(message, innerException) { }
}

public class UserNotFoundException : Exception, ICustomException
{
    public UserNotFoundException(string message) : base(message) { }
    public UserNotFoundException() { }
    public UserNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}

public class UserAlreadyExistsException : Exception, ICustomException
{
    public UserAlreadyExistsException(string message) : base(message) { }
    public UserAlreadyExistsException() { }
    public UserAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException) { }
}

