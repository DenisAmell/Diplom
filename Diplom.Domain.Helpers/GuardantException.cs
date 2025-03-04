namespace Diplom.Domain.Helpers;

public sealed class GuardantException:
    Exception
{

    public GuardantException(
        string message,
        Exception? innerException = null)
        : base(message, innerException)
    {
        
    }
    
    
}