using System.Collections.Generic;

namespace DrifterApps.Holefeeder.Business.Entities
{
    public class StatisticsEntity<T>
    {
        public T Item { get; }
        public IList<SeriesEntity> Yearly { get; }
        public IList<SeriesEntity> Monthly { get; }
        public IList<SeriesEntity> Period { get; }

        public StatisticsEntity(T item, IEnumerable<SeriesEntity> yearly, IEnumerable<SeriesEntity> monthly, IEnumerable<SeriesEntity> period)
        {
            Item = item;
            Yearly = new List<SeriesEntity>(yearly);
            Monthly = new List<SeriesEntity>(monthly);
            Period = new List<SeriesEntity>(period);
        }
    }
}