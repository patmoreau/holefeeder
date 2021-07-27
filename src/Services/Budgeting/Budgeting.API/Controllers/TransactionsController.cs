using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.API.Authorization;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.Transactions.Commands;
using DrifterApps.Holefeeder.Budgeting.Application.Transactions.Queries;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace DrifterApps.Holefeeder.Budgeting.API.Controllers
{
    [Authorize]
    [Route("api/v2/[controller]")]
    [RequiredScope(Scopes.REGISTERED_USER)]
    public class TransactionsController : ControllerBase
    {
        private struct Routes
        {
            public const string GET_TRANSACTIONS = "get-transactions";
            public const string GET_TRANSACTION = "get-transaction";
            public const string MAKE_PURCHASE = "make-purchase";
            public const string TRANSFER = "transfer";
        }

        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(Routes.GET_TRANSACTIONS, Name = Routes.GET_TRANSACTIONS)]
        [ProducesResponseType(typeof(QueryResult<TransactionViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetTransactions([FromQuery] int? offset, int? limit, string[] sort,
            string[] filter, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetTransactionsQuery(offset, limit, sort, filter), cancellationToken);

            Response?.Headers.Add("X-Total-Count", $"{result.TotalCount}");

            return Ok(result);
        }

        [HttpGet("{id:guid}", Name = Routes.GET_TRANSACTION)]
        [ProducesResponseType(typeof(TransactionViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetTransaction(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetTransactionQuery { Id = id }, cancellationToken);

            return Ok(result);
        }

        [HttpPost(Routes.MAKE_PURCHASE, Name = Routes.MAKE_PURCHASE)]
        [ProducesResponseType(typeof(CommandResult<Guid>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> MakePurchaseCommand([FromBody] MakePurchaseCommand command,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CommandResult<Guid>.Create(CommandStatus.BadRequest, Guid.Empty,
                    ModelState.Values.Select(x => x.ToString())));
            }

            try
            {
                var result = await _mediator.Send(command, cancellationToken);

                if (result.Status != CommandStatus.Created)
                {
                    return BadRequest(result);
                }

                return CreatedAtRoute(Routes.GET_TRANSACTION, new { id = result.Result }, result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(CommandResult<Guid>.Create(CommandStatus.BadRequest, Guid.Empty,
                    ex.Errors.Select(e => e.ToString())));
            }
        }

        [HttpPost(Routes.TRANSFER, Name = Routes.TRANSFER)]
        [ProducesResponseType(typeof(CommandResult<Guid>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> TransferCommand([FromBody] TransferCommand command,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CommandResult<Guid>.Create(CommandStatus.BadRequest, Guid.Empty,
                    ModelState.Values.Select(x => x.ToString())));
            }

            try
            {
                var result = await _mediator.Send(command, cancellationToken);

                if (result.Status != CommandStatus.Created)
                {
                    return BadRequest(result);
                }

                return CreatedAtRoute(Routes.GET_TRANSACTION, new { id = result.Result }, result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(CommandResult<Guid>.Create(CommandStatus.BadRequest, Guid.Empty,
                    ex.Errors.Select(e => e.ToString())));
            }
        }
    }
}
