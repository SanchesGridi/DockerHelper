namespace DockerHelper.Core.Exceptions;

public class ExceptionWithHint : Exception
{
    public ExceptionWithHint(string messageHint) : base(messageHint)
    {
    }

    public ExceptionWithHint(string messageHint, Exception innerException) : base(messageHint, innerException)
    {
    }
}
