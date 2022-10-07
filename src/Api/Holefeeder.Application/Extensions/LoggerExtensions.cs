using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Holefeeder.Application.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "API Request: {Name} {Request}")]
    public static partial void LogMediatrRequest(this ILogger logger, string name, IBaseRequest request);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Information,
        Message = "API Request: {Name} with result {Response}")]
    public static partial void LogMediatrResponse(this ILogger logger, string name, object? response);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Information,
        Message = "{Command} {Account}")]
    public static partial void LogAccount(this ILogger logger, string command, Account account);

    [LoggerMessage(
        EventId = 1004,
        Level = LogLevel.Information,
        Message = "{Command} {Category}")]
    public static partial void LogCategory(this ILogger logger, string command, Category category);

    [LoggerMessage(
        EventId = 1005,
        Level = LogLevel.Information,
        Message = "{Command} {Cashflow}")]
    public static partial void LogCashflow(this ILogger logger, string command, Cashflow cashflow);

    [LoggerMessage(
        EventId = 1006,
        Level = LogLevel.Information,
        Message = "{Command} {Transaction}")]
    public static partial void LogTransaction(this ILogger logger, string command, Transaction transaction);

    [LoggerMessage(
        EventId = 1007,
        Level = LogLevel.Information,
        Message = "{DatabaseName} - Migration attempt #{TryCount}")]
    public static partial void LogMigrationAttempt(this ILogger logger, string databaseName, int tryCount);

    [LoggerMessage(
        EventId = 1008,
        Level = LogLevel.Information,
        Message = "{DatabaseName} - Migration completed successfully")]
    public static partial void LogMigrationSuccess(this ILogger logger, string databaseName);

    [LoggerMessage(
        EventId = 9000,
        Level = LogLevel.Error,
        Message = "{DatabaseName} - Migration attempt #{TryCount}")]
    public static partial void LogMigrationError(this ILogger logger, string databaseName, int tryCount,
        Exception exception);
}
