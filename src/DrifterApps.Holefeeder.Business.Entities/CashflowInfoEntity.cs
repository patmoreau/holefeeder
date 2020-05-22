using System;

namespace DrifterApps.Holefeeder.Business.Entities
{
    public class CashflowInfoEntity : BaseEntity, IIdentityEntity<CashflowInfoEntity>
    {
        public DateTime? Date { get; }

        public CashflowInfoEntity(string id, DateTime? date) : base (id)
        {
            Date = date;
        }

        public CashflowInfoEntity With(string id = null, DateTime? date = null) =>
            new CashflowInfoEntity(id ?? Id, date ?? Date);

        public CashflowInfoEntity WithId(string id) => With(id);
    }
}
