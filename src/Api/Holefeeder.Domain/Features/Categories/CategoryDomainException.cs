namespace Holefeeder.Domain.Features.Categories;

#pragma warning disable CA1032
public class CategoryDomainException : DomainException<Category>
#pragma warning restore CA1032
{
    public CategoryDomainException(string message) : base(message)
    {
    }

    public CategoryDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
