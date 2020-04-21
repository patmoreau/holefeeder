using System;

namespace DrifterApps.Holefeeder.Services.Dto
{
    public class SeriesDto
    {
        public DateTime From { get; }
        public DateTime To { get; }
        public int Count { get; }
        public decimal Amount { get; }

        public SeriesDto(DateTime from, DateTime to, int count, decimal amount)
        {
            From = from;
            To = to;
            Count = count;
            Amount = amount;
        }
    }
}
