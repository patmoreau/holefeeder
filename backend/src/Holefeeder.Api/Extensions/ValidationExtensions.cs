using FluentValidation;

namespace Holefeeder.Api.Extensions;

internal static class ValidationExtensions
{
    internal static IDictionary<string, string[]> ToDictionary(this ValidationException exception) =>
        exception.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );
}
