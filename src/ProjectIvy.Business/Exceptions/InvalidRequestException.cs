namespace ProjectIvy.Business.Exceptions;

public class InvalidRequestException : Exception
{
    public InvalidRequestException(string message) : base(message)
    {
    }
}
