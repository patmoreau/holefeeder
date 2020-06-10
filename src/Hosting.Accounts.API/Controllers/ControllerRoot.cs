using System;
using DrifterApps.Holefeeder.Hosting.Security;
using Microsoft.AspNetCore.Mvc;

namespace DrifterApps.Holefeeder.API.Controllers
{
    public abstract class ControllerRoot : ControllerBase
    {
        protected Guid UserId =>
            Guid.Parse(User.FindFirst(HolefeederClaimTypes.HOLEFEEDER_ID)?.Value ?? Guid.Empty.ToString());
    }
}
