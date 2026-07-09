using DrifterApps.Seeds.Application.Extensions;
using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.UseCases;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Domain.Features.Transactions;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.Application.Extensions;

public static class ResultErrorExtensions
{
    internal static IResult ToProblem(this ResultError error) =>
        error switch
        {
            ResultErrorAggregate validationError => validationError.ToValidationProblemDetails(),
            { Code: AccountErrors.CodeNotFound } => error.ToProblemDetails(StatusCodes.Status404NotFound),
            { Code: AccountErrors.CodeIdRequired } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: AccountErrors.CodeNameRequired } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: AccountErrors.CodeOpenDateRequired } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: AccountErrors.CodeUserIdRequired } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: AccountErrors.CodeAccountClosed } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: AccountErrors.CodeActiveCashflows } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: AccountErrors.CodeNameAlreadyExists } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: CashflowErrors.CodeIdRequired } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: CashflowErrors.CodeEffectiveDateRequired } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: CashflowErrors.CodeFrequencyInvalid } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: CashflowErrors.CodeRecurrenceInvalid } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: CashflowErrors.CodeAccountIdRequired } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: CashflowErrors.CodeCategoryIdRequired } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: CashflowErrors.CodeUserIdRequired } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: CashflowErrors.CodeAlreadyInactive } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: CashflowErrors.CodeNotFound } => error.ToProblemDetails(StatusCodes.Status404NotFound),
            { Code: CategoryErrors.CodeIdRequired } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: CategoryErrors.CodeNameRequired } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: CategoryErrors.CodeUserIdRequired } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: StoreItemErrors.CodeCodeAlreadyExists } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: StoreItemErrors.CodeNotFound } => error.ToProblemDetails(StatusCodes.Status404NotFound),
            { Code: TransactionErrors.CodeIdRequired } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: TransactionErrors.CodeDateRequired } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: TransactionErrors.CodeFrequencyInvalid } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: TransactionErrors.CodeRecurrenceInvalid } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: TransactionErrors.CodeAccountIdRequired } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: TransactionErrors.CodeCategoryIdRequired } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: TransactionErrors.CodeUserIdRequired } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: TransactionErrors.CodeCashflowRequired } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: TransactionErrors.CodeNotFound } => error.ToProblemDetails(StatusCodes.Status404NotFound),
            { Code: TransactionErrors.CodeAccountNotFound } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: TransactionErrors.CodeCategoryNotFound } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: TransactionErrors.CodeCategoryNameNotFound } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            { Code: SyncErrors.CodeTypeInvalid } => error.ToProblemDetails(StatusCodes.Status400BadRequest),
            _ => error.ToProblemDetails(StatusCodes.Status500InternalServerError)
        };
}
