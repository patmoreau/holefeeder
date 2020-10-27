using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Transactions.Models;
using DrifterApps.Holefeeder.Application.Transactions.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hosting.Transactions.API.Controllers
{
    // [Authorize(Policy = Policies.REGISTERED_USER)]
    [ApiController]
    [Route("api/v2/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private struct Routes
        {
            public const string GET_CATEGORIES = "get_catgories";
        }

        private readonly IMediator _mediator;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(IMediator mediator, ILogger<CategoriesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet(Name = Routes.GET_CATEGORIES)]
        [ProducesResponseType(typeof(CategoryViewModel[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetCategoriesAsync(CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(new GetCategoriesQuery(), cancellationToken);

            return Ok(response);
        }
    }
}
