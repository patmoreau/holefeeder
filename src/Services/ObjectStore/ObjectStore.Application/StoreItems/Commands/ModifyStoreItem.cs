using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;
using DrifterApps.Holefeeder.ObjectStore.Domain.Exceptions;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.Extensions.Logging;

using OneOf;

namespace DrifterApps.Holefeeder.ObjectStore.Application.StoreItems.Commands;

public static class ModifyStoreItem
{
    public record Request(Guid Id, string Data)
        : IRequest<OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult>>;

    public class Validator : AbstractValidator<Request>,
        IValidator<Request, OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult>>
    {
        public Validator(ILogger<Validator> logger)
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Data).NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

        public OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult>
            CreateResponse(ValidationResult result) => new ValidationErrorsRequestResult(result.ToDictionary());
    }

    public class Handler : IRequestHandler<Request,
        OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult>>
    {
        private readonly IStoreItemsRepository _itemsRepository;
        private readonly ItemsCache _cache;
        private readonly ILogger _logger;

        public Handler(IStoreItemsRepository itemsRepository, ItemsCache cache, ILogger<Handler> logger)
        {
            _itemsRepository = itemsRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult>>
            Handle(Request request,
                CancellationToken cancellationToken)
        {
            try
            {
                var storeItem =
                    await _itemsRepository.FindByIdAsync((Guid)_cache["UserId"], request.Id, cancellationToken);
                if (storeItem is null)
                {
                    return new NotFoundRequestResult();
                }

                storeItem = storeItem with {Data = request.Data};

                _logger.LogInformation("----- Modify Store Item - StoreItem: {@StoreItem}", storeItem);

                await _itemsRepository.SaveAsync(storeItem, cancellationToken);

                await _itemsRepository.UnitOfWork.CommitAsync(cancellationToken);

                return Unit.Value;
            }
            catch (ObjectStoreDomainException ex)
            {
                _itemsRepository.UnitOfWork.Dispose();
                return new DomainErrorRequestResult(ex.Context, ex.Message);
            }
        }
    }
}
