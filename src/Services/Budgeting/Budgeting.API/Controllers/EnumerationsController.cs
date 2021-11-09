using System.Net;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Enumerations.Queries;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace DrifterApps.Holefeeder.Budgeting.API.Controllers
{
    [ApiVersion("2.0")]
    public class EnumerationsController : ControllerRoot
    {
        private struct Routes
        {
            public const string GET_ACCOUNT_TYPES = "get-account-types";
            public const string GET_CATEGORY_TYPES = "get-category-types";
            public const string GET_DATE_INTERVAL_TYPES = "get-date-interval-types";
        }

        private readonly IMediator _mediator;

        public EnumerationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(Routes.GET_ACCOUNT_TYPES, Name = Routes.GET_ACCOUNT_TYPES)]
        [ProducesResponseType(typeof(AccountType[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAccountTypes(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAccountTypesQuery(), cancellationToken);

            return Ok(result);
        }

        [HttpGet(Routes.GET_CATEGORY_TYPES, Name = Routes.GET_CATEGORY_TYPES)]
        [ProducesResponseType(typeof(CategoryType[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetCategoryTypes(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCategoryTypesQuery(), cancellationToken);

            return Ok(result);
        }

        [HttpGet(Routes.GET_DATE_INTERVAL_TYPES, Name = Routes.GET_DATE_INTERVAL_TYPES)]
        [ProducesResponseType(typeof(DateIntervalType[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetDateIntervalTypes(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetDateIntervalTypesQuery(), cancellationToken);

            return Ok(result);
        }
    }
}
