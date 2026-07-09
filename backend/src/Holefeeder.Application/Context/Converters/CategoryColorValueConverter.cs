using Holefeeder.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Holefeeder.Application.Context.Converters;

public class CategoryColorValueConverter()
    : ValueConverter<CategoryColor, string>(color => color.ToString(), value => CategoryColor.Create(value).Value);
