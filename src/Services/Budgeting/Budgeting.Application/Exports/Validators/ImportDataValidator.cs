using DrifterApps.Holefeeder.Budgeting.Application.Exports.Commands;

using FluentValidation;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Budgeting.Application.Exports.Validators
{
    public class ImportDataValidator : AbstractValidator<ImportDataCommand>
    {
        public ImportDataValidator(ILogger<ImportDataValidator> logger)
        {
            RuleFor(command => command.Data)
                .NotNull()
                .Must(data => 
                    data.RootElement.TryGetProperty("accounts", out _) ||
                    data.RootElement.TryGetProperty("categories", out _) ||
                    data.RootElement.TryGetProperty("cashflows", out _) ||
                    data.RootElement.TryGetProperty("transactions", out _))
                .WithMessage("must contain at least 1 array of accounts|categories|cashflows|transactions");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
