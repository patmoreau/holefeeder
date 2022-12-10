using Holefeeder.Domain.Features.StoreItem;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Holefeeder.Application.Context;

public class StoreItemEntityTypeConfiguration : IEntityTypeConfiguration<StoreItem>
{
    public void Configure(EntityTypeBuilder<StoreItem> builder)
    {
        builder.ToTable("store_items").HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Code).HasColumnName("code").IsRequired();
        builder.Property(e => e.Data).HasColumnName("data");
        builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
    }
}
