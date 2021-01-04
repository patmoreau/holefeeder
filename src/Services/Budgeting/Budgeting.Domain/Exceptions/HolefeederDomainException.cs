using System;

namespace DrifterApps.Holefeeder.Budgeting.Domain.Exceptions
{
    public class HolefeederDomainException : Exception
    {
        public HolefeederDomainException()
        {
        }

        public HolefeederDomainException(string message) : base(message)
        {
        }

        public HolefeederDomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
