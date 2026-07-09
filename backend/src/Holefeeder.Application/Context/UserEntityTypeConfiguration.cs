using DrifterApps.Seeds.Application.Context;

using Holefeeder.Domain.Features.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Holefeeder.Application.Context;

internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .ToTable("users")
            .HasKey(e => e.Id);
        builder
            .Property(e => e.Id)
            .HasColumnName("id")
            .HasConversion<StronglyTypedIdValueConverter<UserId>>()
            .IsRequired();
        builder
            .Property(e => e.Inactive)
            .HasColumnName("inactive")
            .IsRequired();
        builder
            .HasMany(e => e.UserIdentities)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId);
    }
}
