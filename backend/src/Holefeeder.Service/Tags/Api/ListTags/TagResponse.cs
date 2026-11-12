namespace Holefeeder.Service.Tags.Api.ListTags;

internal sealed record TagResponse(string Tag, int TransactionCount, int ActiveCashflowCount);
