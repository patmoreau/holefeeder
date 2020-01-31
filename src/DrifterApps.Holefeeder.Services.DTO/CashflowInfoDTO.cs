using System;

namespace DrifterApps.Holefeeder.Services.DTO
{
    public class CashflowInfoDTO
    {
        public string Id { get; }

        public DateTime? Date { get; }

        public CashflowInfoDTO(string id, DateTime? date)
        {
            Id = id;
            Date = date;
        }
    }
}