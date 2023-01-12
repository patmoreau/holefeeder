using Holefeeder.Domain.Features.Accounts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Holefeeder.Application.Context;

internal class AccountEntityTypeConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder
            .ToTable("accounts")
            .HasKey(e => e.Id);
        builder
            .HasIndex(e => new {e.Id, e.UserId})
            .IsUnique();
        builder
            .Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();
        builder
            .Property(e => e.Type)
            .HasColumnName("type")
            .IsRequired()
            .HasConversion(p => p.Name, p => AccountType.FromName(p, false));
        builder
            .Property(e => e.Name)
            .HasColumnName("name")
            .IsRequired();
        builder
            .Property(e => e.Favorite)
            .HasColumnName("favorite")
            .IsRequired();
        builder
            .Property(e => e.OpenBalance)
            .HasColumnName("open_balance")
            .IsRequired();
        builder
            .Property(e => e.OpenDate)
            .HasColumnName("open_date")
            .IsRequired();
        builder
            .Property(e => e.Description)
            .HasColumnName("description");
        builder
            .Property(e => e.Inactive)
            .HasColumnName("inactive")
            .IsRequired();
        builder
            .Property(e => e.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        builder
            .HasMany(e => e.Cashflows)
            .WithOne(e => e.Account)
            .HasForeignKey(x => x.AccountId);
        builder
            .HasMany(e => e.Transactions)
            .WithOne(e => e.Account)
            .HasForeignKey(x => x.AccountId);
    }
}
