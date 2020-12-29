using System;

namespace DrifterApps.Holefeeder.ObjectStore.Domain.Exceptions
{
    public class ObjectStoreDomainException : Exception
    {
        public ObjectStoreDomainException()
        {
        }

        public ObjectStoreDomainException(string message)
            : base(message)
        {
        }

        public ObjectStoreDomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
