using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.Business;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;
using DrifterApps.Holefeeder.Common.Authorization;
using DrifterApps.Holefeeder.Common.Extensions;
using DrifterApps.Holefeeder.Services.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Services.API.Controllers
{
    [Route("api/v1/[controller]"), Authorize(Policy = Policies.RegisteredUser)]
    public class UsersController : Controller
    {
        private struct Routes
        {
            public const string GET_USERS = "get_users";
            public const string GET_USER = "get_user";
            public const string PUT_USER = "put_user";
            public const string POST_USER = "post_user";
            public const string DELETE_USER = "delete_user";
        }

        private readonly IUsersService _service;
        private readonly IMapper _mapper;

        public UsersController(IUsersService service, IMapper mapper)
        {
            _service = service.ThrowIfNull(nameof(service));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        [HttpGet(Name = Routes.GET_USERS)]
        [ProducesResponseType(typeof(UserDTO[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
        {
            return Ok(await _service.FindAsync(new QueryParams(), cancellationToken).ConfigureAwait(false));
        }

        [HttpGet("{id}", Name = Routes.GET_USER)]
        [ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAsync([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            var result = await _service.FindByIdAsync(id, cancellationToken).ConfigureAwait(false);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<UserDTO>(result));
        }

        [HttpPost(Name = Routes.POST_USER), Authorize(Policy = Policies.AuthenticatedUser)]
        [ProducesResponseType(typeof(ObjectDataDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> PostAsync(CancellationToken cancellationToken = default)
        {
            var model = new UserDTO(
                null,
                User.FindFirst(JwtRegisteredClaimNames.GivenName)?.Value,
                User.FindFirst(JwtRegisteredClaimNames.FamilyName)?.Value,
                User.FindFirst(JwtRegisteredClaimNames.Email)?.Value,
                User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value,
                DateTime.Now.Date);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _service.FindByEmailAsync(model.EmailAddress, cancellationToken).ConfigureAwait(false) != null)
            {
                ModelState.AddModelError("duplicateKey", $"Email address '{model.EmailAddress}' is already registered");
                return BadRequest(ModelState);
            }

            var result = await _service.CreateAsync(_mapper.Map<UserEntity>(model), cancellationToken).ConfigureAwait(false);

            return CreatedAtRoute(Routes.GET_USER, new { id = result.Id }, _mapper.Map<UserDTO>(result));
        }

        [HttpPut("{id}", Name = Routes.PUT_USER)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> PutAsync([FromRoute] string id, [FromBody] UserDTO model, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;

            if (!id.Equals(userId, StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.UpdateAsync(id, _mapper.Map<UserEntity>(model), cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpDelete("{id}", Name = Routes.DELETE_USER)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;

            if (!id.Equals(userId, StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized();
            }

            var user = await _service.FindByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }
    }
}