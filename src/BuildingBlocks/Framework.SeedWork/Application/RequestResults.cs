using System;
using System.Collections;
using System.Collections.Generic;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application;

public record NotFoundRequestResult : IRequestResult;

public record ValidationErrorsRequestResult(IDictionary<string, string[]> Errors) : IRequestResult;

public record DomainErrorRequestResult(string Context, string Message) : IRequestResult;

public record ListRequestResult(int Total, IEnumerable Items) : IRequestResult;

public record IdRequestResult(object Item) : IRequestResult;

public record CreatedRequestResult(Guid Id, string Name) : IRequestResult;

public record NoContentResult : IRequestResult;
