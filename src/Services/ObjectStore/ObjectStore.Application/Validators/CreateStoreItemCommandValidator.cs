using System;

using DrifterApps.Holefeeder.ObjectStore.Application.Commands;
using DrifterApps.Holefeeder.ObjectStore.Application.Contracts;

using FluentValidation;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Validators
{
    public class CreateStoreItemCommandValidator : AbstractValidator<CreateStoreItemCommand>
    {
        public CreateStoreItemCommandValidator(IStoreQueriesRepository repository, ItemsCache cache)
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .Must(code => !repository.CodeExistsAsync((Guid)cache["UserId"], code).Result)
                .WithMessage(command => $"Code '{command.Code}' already exists.");
            RuleFor(x => x.Data)
                .NotEmpty();
        }
    }}
