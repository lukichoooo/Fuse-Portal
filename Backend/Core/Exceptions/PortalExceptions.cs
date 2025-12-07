namespace Core.Exceptions;


public class SubjectNotFoundException : Exception, ICustomException
{
    public SubjectNotFoundException(string message) : base(message) { }
    public SubjectNotFoundException() { }
    public SubjectNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}


public class ScheduleNotFoundException : Exception, ICustomException
{
    public ScheduleNotFoundException(string message) : base(message) { }
    public ScheduleNotFoundException() { }
    public ScheduleNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}


public class LecturerNotFoundException : Exception, ICustomException
{
    public LecturerNotFoundException(string message) : base(message) { }
    public LecturerNotFoundException() { }
    public LecturerNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}


public class TestNotFoundException : Exception, ICustomException
{
    public TestNotFoundException(string message) : base(message) { }
    public TestNotFoundException() { }
    public TestNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}
