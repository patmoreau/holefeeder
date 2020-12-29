using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    [SuppressMessage("ReSharper", "CollectionNeverQueried.Global")]
    public class StatisticsDto<T>
    {
        public T Item { get; set; }
        public IList<SeriesDto> Yearly { get; set; }
        public IList<SeriesDto> Monthly { get; set; }
        public IList<SeriesDto> Period { get; set; }
    }
}
