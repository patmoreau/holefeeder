using System;
using System.Collections.Generic;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas
{
    public class TransactionSchema : BaseSchema, IOwnedSchema
    {
        public const string SCHEMA = "transactions";

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }

        public string Account { get; set; }

        public string Category { get; set; }

        public string Cashflow { get; set; }

        public DateTime? CashflowDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "MongoDB needs to assign it")]
        public IList<string> Tags { get; set; }

        public Guid UserId { get; set; }
    }
}
