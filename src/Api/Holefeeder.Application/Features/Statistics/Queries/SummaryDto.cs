namespace Holefeeder.Application;

public record class SummaryDto(SummaryValue Last, SummaryValue Current, SummaryValue Average);
