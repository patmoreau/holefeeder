using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Transactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Holefeeder.Application.Context;

internal class CashflowEntityTypeConfiguration : IEntityTypeConfiguration<Cashflow>
{
    private const char Delimiter = ',';

    public void Configure(EntityTypeBuilder<Cashflow> builder)
    {
        builder
            .ToTable("cashflows")
            .HasKey(e => e.Id);
        builder
            .Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();
        builder
            .Property(e => e.EffectiveDate)
            .HasColumnName("effective_date")
            .IsRequired();
        builder
            .Property(e => e.Amount)
            .HasColumnName("amount")
            .HasPrecision(19, 2)
            .IsRequired();
        builder
            .Property(e => e.IntervalType)
            .HasColumnName("interval_type")
            .HasMaxLength(100)
            .HasConversion(p => p.Name, p => DateIntervalType.FromName(p, false))
            .IsRequired();
        builder
            .Property(e => e.Frequency)
            .HasColumnName("frequency")
            .IsRequired();
        builder
            .Property(e => e.Recurrence)
            .HasColumnName("recurrence")
            .IsRequired();
        builder
            .Property(e => e.Description)
            .HasColumnName("description");
        builder
            .Property(e => e.AccountId)
            .HasColumnName("account_id")
            .IsRequired();
        builder
            .Property(e => e.CategoryId)
            .HasColumnName("category_id")
            .IsRequired();
        builder
            .Property(e => e.Inactive)
            .HasColumnName("inactive")
            .IsRequired();
        builder
            .Property(e => e.Tags)
            .HasColumnName("tags")
            .HasConversion(p => string.Join(Delimiter, p),
                p => string.IsNullOrWhiteSpace(p)
                    ? Array.Empty<string>()
                    : p.Split(Delimiter, StringSplitOptions.TrimEntries));
        builder
            .Property(e => e.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        builder
            .HasOne(c => c.Account)
            .WithMany(e => e.Cashflows)
            .HasForeignKey(x => x.AccountId);
        builder.HasOne(c => c.Category);
        builder.HasMany(c => c.Transactions)
            .WithOne(c => c.Cashflow)
            .HasForeignKey(x => x.CashflowId);
    }
}
