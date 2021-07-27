using System;
using System.Runtime.Serialization;

namespace DrifterApps.Holefeeder.ObjectStore.Domain.Exceptions
{
    [Serializable]
    public class ObjectStoreDomainException : Exception
    {
        public ObjectStoreDomainException(string message) : base(message)
        {
        }

        protected ObjectStoreDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
