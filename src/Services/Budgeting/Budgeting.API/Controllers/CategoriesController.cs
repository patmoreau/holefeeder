using System.Net;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.API.Authorization;
using DrifterApps.Holefeeder.Budgeting.Application.Categories.Queries;
using DrifterApps.Holefeeder.Budgeting.Application.Models;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace DrifterApps.Holefeeder.Budgeting.API.Controllers
{
    [Authorize]
    [Route("api/v2/[controller]")]
    [RequiredScope(Scopes.REGISTERED_USER)]
    public class CategoriesController : ControllerBase
    {
        private struct Routes
        {
            public const string GET_CATEGORIES = "get-categories";
        }

        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(Routes.GET_CATEGORIES, Name = Routes.GET_CATEGORIES)]
        [ProducesResponseType(typeof(CategoryViewModel[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCategoriesQuery(), cancellationToken);

            return Ok(result);
        }
    }
}
