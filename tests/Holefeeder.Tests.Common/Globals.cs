using System.Text.Json;

using DrifterApps.Seeds.Application.Converters;

using Holefeeder.Application.Converters;

namespace Holefeeder.Tests.Common;

public static class Globals
{
    static Globals()
    {
        JsonSerializerOptions.Converters.Add(new StronglyTypedIdJsonConverterFactory());
        JsonSerializerOptions.Converters.Add(new MoneyJsonConverterFactory());
        JsonSerializerOptions.Converters.Add(new CategoryColorJsonConverterFactory());
    }

    public static JsonSerializerOptions JsonSerializerOptions { get; } = new(JsonSerializerDefaults.Web);
}
