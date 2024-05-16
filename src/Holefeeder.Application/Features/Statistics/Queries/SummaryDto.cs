namespace Holefeeder.Application.Features.Statistics.Queries;

public record SummaryDto(SummaryValue Last, SummaryValue Current, SummaryValue Average);
