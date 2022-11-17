using FluentValidation;

namespace Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem;

internal class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Data).NotEmpty();
    }
}
