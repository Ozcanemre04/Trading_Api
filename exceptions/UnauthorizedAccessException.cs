namespace trading_app.exceptions;
public class UnauthorizedAccessException : Exception
{
    public UnauthorizedAccessException(string msg) : base(msg){}    
}
