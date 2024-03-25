namespace Hierarchy.Exceptions;

public class DeleteRootException:Exception
{
    public DeleteRootException()
    {
        
    }

    public DeleteRootException(string message):base(message)
    {
        
    }

    public DeleteRootException(string message, Exception innerException):base(message,innerException)
    {
        
    }
}