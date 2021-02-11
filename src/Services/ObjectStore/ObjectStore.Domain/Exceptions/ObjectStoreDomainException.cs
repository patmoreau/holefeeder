using System;

namespace DrifterApps.Holefeeder.ObjectStore.Domain.Exceptions
{
    public class ObjectStoreDomainException : Exception
    {
        public ObjectStoreDomainException(string message)
            : base(message)
        {
        }
    }
}
