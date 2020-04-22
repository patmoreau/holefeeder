using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    [SuppressMessage("ReSharper", "CollectionNeverQueried.Global")]
    public class StatisticsDto<T>
    {
        public T Item { get; }
        public IList<SeriesDto> Yearly { get; }
        public IList<SeriesDto> Monthly { get; }
        public IList<SeriesDto> Period { get; }

        public StatisticsDto(T item, IEnumerable<SeriesDto> yearly, IEnumerable<SeriesDto> monthly, IEnumerable<SeriesDto> period)
        {
            Item = item;
            Yearly = new List<SeriesDto>(yearly);
            Monthly = new List<SeriesDto>(monthly);
            Period = new List<SeriesDto>(period);
        }
    }
}
