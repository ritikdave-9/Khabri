using System;

namespace Common.Exceptions
{
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException() { }

        public AlreadyExistsException(string message) : base(message) { }

        public AlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
