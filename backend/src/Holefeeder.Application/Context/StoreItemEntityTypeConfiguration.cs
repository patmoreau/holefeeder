using DrifterApps.Seeds.Application.Context;

using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Domain.Features.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Holefeeder.Application.Context;

internal class StoreItemEntityTypeConfiguration : IEntityTypeConfiguration<StoreItem>
{
    public void Configure(EntityTypeBuilder<StoreItem> builder)
    {
        builder.ToTable("store_items").HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .HasConversion<StronglyTypedIdValueConverter<StoreItemId>>()
            .IsRequired();

        builder
            .Property(e => e.Code)
            .HasColumnName("code")
            .IsRequired();

        builder
            .Property(e => e.Data)
            .HasColumnName("data");

        builder
            .Property(e => e.UserId)
            .HasColumnName("user_id")
            .HasConversion<StronglyTypedIdValueConverter<UserId>>()
            .IsRequired();
    }
}
