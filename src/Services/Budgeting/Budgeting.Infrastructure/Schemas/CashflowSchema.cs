using System;
using System.Collections.Generic;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas
{
    public class CashflowSchema : BaseSchema, IOwnedSchema
    {
        public const string SCHEMA = "cashflows";

        public DateTime EffectiveDate { get; set; }

        public decimal Amount { get; set; }

        public DateIntervalType IntervalType { get; set; }

        public int Frequency { get; set; }

        public int Recurrence { get; set; }

        public string Description { get; set; }

        public string Account { get; set; }

        public string Category { get; set; }

        public bool Inactive { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "MongoDB needs to assign it")]
        public IList<string> Tags { get; set; }

        public Guid UserId { get; set; }
    }
}
