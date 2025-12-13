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


public class SylabusNotFoundException : Exception, ICustomException
{
    public SylabusNotFoundException(string message) : base(message) { }
    public SylabusNotFoundException() { }
    public SylabusNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}


public class ExamNotFoundException : Exception, ICustomException
{
    public ExamNotFoundException(string message) : base(message) { }
    public ExamNotFoundException() { }
    public ExamNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}
