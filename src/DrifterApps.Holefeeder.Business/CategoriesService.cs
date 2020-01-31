using DrifterApps.Holefeeder.ResourcesAccess;
using DrifterApps.Holefeeder.Business.Entities;

namespace DrifterApps.Holefeeder.Business
{
    public class CategoriesService : BaseOwnedService<CategoryEntity>, ICategoriesService
    {
        public CategoriesService(ICategoriesRepository repository) : base(repository)
        {
        }
    }
}