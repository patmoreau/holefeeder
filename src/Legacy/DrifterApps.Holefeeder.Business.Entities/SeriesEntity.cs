using System;

namespace DrifterApps.Holefeeder.Business.Entities
{
    public class SeriesEntity
    {
        public DateTime From { get; }
        public DateTime To { get; }
        public int Count { get; }
        public decimal Amount { get; }

        public SeriesEntity(DateTime from, DateTime to, int count, decimal amount)
        {
            From = from;
            To = to;
            Count = count;
            Amount = amount;
        }
    }
}