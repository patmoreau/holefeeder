using DrifterApps.Holefeeder.ObjectStore.Application.Commands;

using FluentValidation;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Validators
{
    public class ModifyStoreItemCommandValidator : AbstractValidator<ModifyStoreItemCommand>
    {
        public ModifyStoreItemCommandValidator(ILogger<ModifyStoreItemCommandValidator> logger)
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Data).NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
