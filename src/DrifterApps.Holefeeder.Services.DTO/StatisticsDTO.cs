using System.Collections.Generic;

namespace DrifterApps.Holefeeder.Services.DTO
{
    public class StatisticsDTO<T>
    {
        public T Item { get; }
        public IList<SeriesDTO> Yearly { get; }
        public IList<SeriesDTO> Monthly { get; }
        public IList<SeriesDTO> Period { get; }

        public StatisticsDTO(T item, IEnumerable<SeriesDTO> yearly, IEnumerable<SeriesDTO> monthly, IEnumerable<SeriesDTO> period)
        {
            Item = item;
            Yearly = new List<SeriesDTO>(yearly);
            Monthly = new List<SeriesDTO>(monthly);
            Period = new List<SeriesDTO>(period);
        }
    }
}