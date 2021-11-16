using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.StoreItems.Queries;
using DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.ObjectStore.Application.StoreItems.Commands;

public static class CreateStoreItem
{
    public record Request(string Code, string Data) : IRequest<IRequestResult>, IValidateable;

    public class Validator : AbstractValidator<Request>
    {
        public Validator(ILogger<Request> logger)
        {
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x => x.Data).NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }

    public class Handler : IRequestHandler<Request, IRequestResult>
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

        public async Task<IRequestResult> Handle(Request request, CancellationToken cancellationToken)
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

                return new CreatedRequestResult(storeItem.Id, nameof(GetStoreItem));
            }
            catch (Exception)
            {
                _itemsRepository.UnitOfWork.Dispose();
                throw;
            }
        }
    }
}
