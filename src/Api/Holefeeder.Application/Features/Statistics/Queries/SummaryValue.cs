namespace Holefeeder.Application;

public record class SummaryValue(decimal Gains, decimal Expenses)
{
    public static SummaryValue Empty { get; set; } = new(0, 0);
}
