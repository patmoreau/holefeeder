using System;

using OneOf;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application.BackgroundRequest;

public class RequestResponse : OneOfBase<Guid, ValidationErrorsRequestResult>
{
    public RequestResponse(OneOf<Guid, ValidationErrorsRequestResult> input) : base(input)
    {
    }
}
