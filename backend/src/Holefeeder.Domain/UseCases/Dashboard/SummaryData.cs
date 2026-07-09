using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.UseCases.Dashboard;

public record SummaryData(CategoryType CategoryType, DateOnly Date, Money Total);
