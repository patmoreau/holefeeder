namespace Holefeeder.Application.Features.Statistics.Queries;

public record SummaryValue(decimal Gains, decimal Expenses)
{
    public static SummaryValue Empty { get; set; } = new(0, 0);
}
