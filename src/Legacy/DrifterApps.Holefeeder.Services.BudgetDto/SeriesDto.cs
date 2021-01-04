using System;

namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    public class SeriesDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Count { get; set; }
        public decimal Amount { get; set; }
    }
}
