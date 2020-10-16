using System.Collections.Generic;
using DrifterApps.Holefeeder.Domain.SeedWork;

namespace DrifterApps.Holefeeder.Domain.AggregatesModel.CashflowAggregate
{
    public class DateIntervalType : Enumeration
    {
        public static DateIntervalType Weekly = new DateIntervalType(1, nameof(Weekly));
        public static DateIntervalType Monthly = new DateIntervalType(2, nameof(Monthly));
        public static DateIntervalType Yearly = new DateIntervalType(3, nameof(Yearly));
        public static DateIntervalType OneTime = new DateIntervalType(4, nameof(OneTime));

        private DateIntervalType(int id, string name) : base(id, name)
        {
        }
    }
}
