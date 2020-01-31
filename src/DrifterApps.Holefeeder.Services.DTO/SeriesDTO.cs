using System;

namespace DrifterApps.Holefeeder.Services.DTO
{
    public class SeriesDTO
    {
        public DateTime From { get; }
        public DateTime To { get; }
        public int Count { get; }
        public decimal Amount { get; }

        public SeriesDTO(DateTime from, DateTime to, int count, decimal amount)
        {
            From = from;
            To = to;
            Count = count;
            Amount = amount;
        }
    }
}