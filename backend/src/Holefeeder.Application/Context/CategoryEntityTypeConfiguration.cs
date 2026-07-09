using DrifterApps.Seeds.Application.Context;

using Holefeeder.Application.Context.Converters;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Holefeeder.Application.Context;

internal class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder
            .ToTable("categories");
        builder
            .Property(e => e.Id)
            .HasColumnName("id")
            .HasConversion<StronglyTypedIdValueConverter<CategoryId>>()
            .IsRequired();
        builder
            .Property(e => e.Type)
            .HasColumnName("type")
            .HasConversion(p => p.Name, p => CategoryType.FromName(p, false))
            .IsRequired();
        builder
            .Property(e => e.Name)
            .HasColumnName("name")
            .IsRequired();
        builder
            .Property(e => e.Color)
            .HasColumnName("color")
            .HasConversion<CategoryColorValueConverter>()
            .IsRequired();
        builder
            .Property(e => e.Favorite)
            .HasColumnName("favorite")
            .IsRequired();
        builder
            .Property(e => e.System)
            .HasColumnName("system")
            .IsRequired();
        builder
            .Property(e => e.BudgetAmount)
            .HasColumnName("budget_amount")
            .HasConversion<MoneyValueConverter>()
            .IsRequired();
        builder
            .Property(e => e.UserId)
            .HasColumnName("user_id")
            .HasConversion<StronglyTypedIdValueConverter<UserId>>()
            .IsRequired();
    }
}
