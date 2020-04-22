using System;

namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    public class CashflowInfoDto
    {
        public string Id { get; }

        public DateTime? Date { get; }

        public CashflowInfoDto(string id, DateTime? date)
        {
            Id = id;
            Date = date;
        }
    }
}
