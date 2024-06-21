using Holefeeder.Domain.Features.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Holefeeder.Application.Context;

internal class UserIdentityEntityTypeConfiguration : IEntityTypeConfiguration<UserIdentity>
{
    public void Configure(EntityTypeBuilder<UserIdentity> builder)
    {
        builder
            .ToTable("user_identities")
            .HasKey(e => new { e.UserId, e.IdentityObjectId });
        builder
            .Property(e => e.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        builder
            .Property(e => e.IdentityObjectId)
            .HasColumnName("identity_object_id")
            .IsRequired();
        builder
            .Property(e => e.Inactive)
            .HasColumnName("inactive")
            .IsRequired();
        builder
            .HasOne(e => e.User)
            .WithMany(e => e.UserIdentities)
            .HasForeignKey(e => e.UserId);
    }
}
