namespace Holefeeder.Application.Features.StoreItems.Commands;

public static class CreateStoreItem
{
    public record Request(string Code, string Data);

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x => x.Data).NotEmpty();
        }
    }
}
