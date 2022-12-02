namespace Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem;

internal class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Data).NotEmpty();
    }
}
