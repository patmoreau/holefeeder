namespace Holefeeder.Application.Features.MyData.Commands.ImportData;

public class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(command => command.Data)
            .NotNull()
            .Must(data =>
                data.Accounts.Any() ||
                data.Categories.Any() ||
                data.Cashflows.Any() ||
                data.Transactions.Any())
            .WithMessage("must contain at least 1 array of accounts|categories|cashflows|transactions");
    }
}
