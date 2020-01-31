using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Business.Entities
{
    public class CategoryInfoEntity : BaseEntity, IIdentityEntity<CategoryInfoEntity>
    {
        public string Name { get; }
        public CategoryType Type { get; }
        public string Color { get; }

        public CategoryInfoEntity(string id, string name, CategoryType type, string color) : base(id)
        {
            Name = name;
            Type = type;
            Color = color;
        }

        public CategoryInfoEntity With(string id = null, string name = null, CategoryType? type = null, string color = null) =>
            new CategoryInfoEntity(id ?? Id, name ?? Name, type ?? Type, color ?? Color);

        public CategoryInfoEntity WithId(string id) => With(id: id);
    }
}