using Holefeeder.Domain.Features.Transactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Holefeeder.Application.Context;

internal class TransactionEntityTypeConfiguration : IEntityTypeConfiguration<Transaction>
{
    private const char DELIMITER = ',';

    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder
            .ToTable("transactions")
            .HasKey(e => e.Id);
        builder
            .Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();
        builder
            .Property(e => e.Date)
            .HasColumnName("date")
            .HasColumnType("DATE")
            .IsRequired();
        builder
            .Property(e => e.Amount)
            .HasColumnName("amount")
            .HasPrecision(19, 2)
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
            .Property(e => e.CashflowId)
            .HasColumnName("cashflow_id");
        builder
            .Property(e => e.CashflowDate)
            .HasColumnName("cashflow_date");
        builder
            .Property(e => e.Tags)
            .HasColumnName("tags").HasConversion(p => string.Join(DELIMITER, p),
                p => string.IsNullOrWhiteSpace(p)
                    ? Array.Empty<string>()
                    : p.Split(DELIMITER, StringSplitOptions.TrimEntries));
        builder
            .Property(e => e.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        builder
            .HasOne(e => e.Account)
            .WithMany(e => e.Transactions)
            .HasForeignKey(e => e.AccountId);
        builder
            .HasOne(e => e.Category);
        builder
            .HasOne(e => e.Cashflow)
            .WithMany(e => e.Transactions)
            .HasForeignKey(e => e.CashflowId);
    }
}
