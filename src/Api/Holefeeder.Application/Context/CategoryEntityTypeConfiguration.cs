using Holefeeder.Domain.Features.Categories;

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
            .IsRequired();
        builder
            .Property(e => e.UserId)
            .HasColumnName("user_id")
            .IsRequired();
    }
}
