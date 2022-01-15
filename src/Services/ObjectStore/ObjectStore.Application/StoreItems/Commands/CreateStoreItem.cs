using System;
using System.Collections.Generic;
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

public static class CreateStoreItem
{
    public record Request(string Code, string Data) :
        IRequest<OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>>,
        IValidateable;

    public class Validator : AbstractValidator<Request>,
        IValidator<Request, OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>>
    {
        public Validator(ILogger<Validator> logger)
        {
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x => x.Data).NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

        public OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult> CreateResponse(
            ValidationResult result) =>
            new ValidationErrorsRequestResult(result.ToDictionary());
    }

    public class Handler : IRequestHandler<Request,
        OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>>
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

        public async Task<OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            if (await _itemsRepository.FindByCodeAsync((Guid)_cache["UserId"], request.Code, cancellationToken) != null)
            {
                return new ValidationErrorsRequestResult(new Dictionary<string, string[]>
                {
                    { nameof(request.Code), new[] { $"Code '{request.Code}' already exists." } }
                });
            }

            try
            {
                var storeItem = StoreItem.Create(request.Code, request.Data, (Guid)_cache["UserId"]);

                _logger.LogInformation("----- Create Store Item - StoreItem: {@StoreItem}", storeItem);

                await _itemsRepository.SaveAsync(storeItem, cancellationToken);

                await _itemsRepository.UnitOfWork.CommitAsync(cancellationToken);

                return storeItem.Id;
            }
            catch (ObjectStoreDomainException ex)
            {
                _itemsRepository.UnitOfWork.Dispose();
                return new DomainErrorRequestResult(ex.Context, ex.Message);
            }
        }
    }
}
