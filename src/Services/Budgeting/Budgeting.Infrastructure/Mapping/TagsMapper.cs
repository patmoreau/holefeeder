using System;
using System.Collections.Generic;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Mapping;

public class TagsMapper
{
    private const char DELIMITER = ',';

    public string Map(IEnumerable<string> tagList) => string.Join(DELIMITER, tagList);

    public string[] Map(string tags) => string.IsNullOrWhiteSpace(tags) ? Array.Empty<string>() : tags.Split(DELIMITER);
}
