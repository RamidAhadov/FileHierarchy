namespace Hierarchy.Exceptions;

public class NodeNotFoundException:Exception
{
    public NodeNotFoundException()
    {
        
    }

    public NodeNotFoundException(string message):base(message)
    {
        
    }

    public NodeNotFoundException(string message,Exception innerException):base(message,innerException)
    {
        
    }
}