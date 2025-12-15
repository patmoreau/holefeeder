namespace Holefeeder.Domain.UseCases.Dashboard;

public record SummaryResult(decimal CurrentExpenses, decimal ExpenseVariation, decimal ExpenseVariationPercentage, decimal NetFlow, decimal CurrentGains, decimal AverageExpenses);
