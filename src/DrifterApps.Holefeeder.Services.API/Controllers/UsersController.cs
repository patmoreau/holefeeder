using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
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
        private class Routes
        {
            public const string GET_USERS = "get_users";
            public const string GET_USER = "get_user";
            public const string PUT_USER = "put_user";
            public const string POST_USER = "post_user";
            public const string DELETE_USER = "delete_user";
        }

        private readonly IUsersService _service;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UsersController(IUsersService service, IMapper mapper, ILogger<UsersController> logger)
        {
            _service = service.ThrowIfNull(nameof(service));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
            _logger = logger.ThrowIfNull(nameof(logger));
        }

        [HttpGet(Name = Routes.GET_USERS)]
        [ProducesResponseType(typeof(UserDTO[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Get()
        {
            return Ok(await _service.FindAsync(new QueryParams()));
        }

        [HttpGet("{id}", Name = Routes.GET_USER)]
        [ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var result = await _service.FindByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost(Name = Routes.POST_USER), Authorize(Policy = Policies.AuthenticatedUser)]
        [ProducesResponseType(typeof(ObjectDataDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Post()
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

            if (await _service.FindByEmailAsync(model.EmailAddress) != null)
            {
                ModelState.AddModelError("duplicateKey", $"Email address '{model.EmailAddress}' is already registered");
                return BadRequest(ModelState);
            }

            var result = await _service.CreateAsync(_mapper.Map<UserEntity>(model));

            return CreatedAtRoute(Routes.GET_USER, new { id = result.Id }, _mapper.Map<UserDTO>(result));
        }

        [HttpPut("{id}", Name = Routes.PUT_USER)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Put([FromRoute] string id, [FromBody] UserDTO model)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;

            if (id != userId)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.UpdateAsync(id, _mapper.Map<UserEntity>(model));

            return NoContent();
        }

        [HttpDelete("{id}", Name = Routes.DELETE_USER)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;

            if (id != userId)
            {
                return Unauthorized();
            }

            var user = await _service.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}