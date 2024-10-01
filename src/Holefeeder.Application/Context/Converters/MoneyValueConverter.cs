using Holefeeder.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Holefeeder.Application.Context.Converters;

public class MoneyValueConverter()
    : ValueConverter<Money, decimal>(money => money.Value, value => Money.Create(value).Value);
