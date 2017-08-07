using System;

namespace AnticevicApi.BL.Exceptions
{
    public class InvalidRequestException : Exception
    {
        public InvalidRequestException(string message) : base(message) { }
    }
}
