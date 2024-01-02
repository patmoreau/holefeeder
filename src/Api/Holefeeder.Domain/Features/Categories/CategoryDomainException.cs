namespace Holefeeder.Domain.Features.Categories;

#pragma warning disable CA1032
public class CategoryDomainException(string message) : DomainException<Category>(message)
#pragma warning restore CA1032
{
}
