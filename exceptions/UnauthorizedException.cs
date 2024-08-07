namespace trading_app.exceptions;
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string msg) : base(msg){}    
}
