using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.Business;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;
using DrifterApps.Holefeeder.Common.Authorization;
using DrifterApps.Holefeeder.Common.Enums;
using DrifterApps.Holefeeder.Common.Extensions;
using DrifterApps.Holefeeder.Services.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrifterApps.Holefeeder.Services.API.Controllers
{
    [Route("api/v1/[controller]"), Authorize(Policy = Policies.REGISTERED_USER)]
    public class CategoriesController : Controller
    {
        private struct Routes
        {
            public const string GET_CATEGORIES = "get_categories";
            public const string GET_CATEGORY = "get_category";
            public const string POST_CATEGORY = "post_category";
            public const string PUT_CATEGORY = "put_category";
            public const string DELETE_CATEGORY = "delete_category";
            public const string GET_STATISTICS = "get_statistics";
            public const string GET_STATISTICS_BY_TAGS = "get_statisticsByTags";
        }

        private readonly ICategoriesService _service;
        private readonly IStatisticsService _statisticsService;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoriesService service, IStatisticsService statisticsService, IMapper mapper)
        {
            _service = service.ThrowIfNull(nameof(service));
            _statisticsService = statisticsService.ThrowIfNull(nameof(statisticsService));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        [HttpGet(Name = Routes.GET_CATEGORIES)]
        [ProducesResponseType(typeof(CategoryDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAsync([FromQuery] int? offset, [FromQuery] int? limit, [FromQuery] string[] sort, [FromQuery] string[] filter, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HOLEFEEDER_ID)?.Value;

            var result = await _service.FindAsync(userId, new QueryParams(offset, limit, sort, filter), cancellationToken).ConfigureAwait(false);
            return Ok(_mapper.Map<CategoryDto[]>(result));
        }

        [HttpGet("{id}", Name = Routes.GET_CATEGORY)]
        [ProducesResponseType(typeof(CategoryDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HOLEFEEDER_ID)?.Value;
            if (!await _service.IsOwnerAsync(userId, id, cancellationToken).ConfigureAwait(false))
            {
                return NotFound();
            }

            var result = await _service.FindByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CategoryDto>(result));
        }

        [HttpPost(Name = Routes.POST_CATEGORY)]
        [ProducesResponseType(typeof(CategoryDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> PostAsync([FromBody] CategoryDto model, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HOLEFEEDER_ID)?.Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.CreateAsync(userId, _mapper.Map<CategoryEntity>(model), cancellationToken).ConfigureAwait(false);

            return CreatedAtRoute(Routes.GET_CATEGORY, new { id = result.Id }, _mapper.Map<CategoryDto>(result));
        }

        [HttpPut("{id}", Name = Routes.PUT_CATEGORY)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> PutAsync([FromRoute] string id, [FromBody] CategoryDto model, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HOLEFEEDER_ID)?.Value;
            if (!await _service.IsOwnerAsync(userId, id, cancellationToken).ConfigureAwait(false))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.UpdateAsync(id, _mapper.Map<CategoryEntity>(model), cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpDelete("{id}", Name = Routes.DELETE_CATEGORY)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HOLEFEEDER_ID)?.Value;
            if (!await _service.IsOwnerAsync(userId, id, cancellationToken).ConfigureAwait(false))
            {
                return NotFound();
            }

            await _service.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpGet("statistics", Name = Routes.GET_STATISTICS)]
        [ProducesResponseType(typeof(StatisticsDto<CategoryInfoDto>[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetStatisticsAsync([FromQuery] DateTime effectiveDate, [FromQuery] DateIntervalType intervalType = DateIntervalType.Monthly, [FromQuery] int frequency = 1, CancellationToken cancellationToken = default)
        {
            if (intervalType == DateIntervalType.OneTime)
            {
                return BadRequest();
            }

            var userId = User.FindFirst(HolefeederClaimTypes.HOLEFEEDER_ID)?.Value;

            var utc = effectiveDate.ToUniversalTime();

            var result = await _statisticsService.StatisticsAsync(userId, new DateTime(utc.Year, utc.Month, utc.Day), intervalType, frequency, cancellationToken).ConfigureAwait(false);

            return Ok(_mapper.Map<StatisticsDto<CategoryInfoDto>[]>(result));
        }

        [HttpGet("{id}/statistics", Name = Routes.GET_STATISTICS_BY_TAGS)]
        [ProducesResponseType(typeof(StatisticsDto<string>[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetByTagsAsync([FromRoute] string id, [FromQuery] DateTime effectiveDate, [FromQuery] DateIntervalType intervalType = DateIntervalType.Monthly, [FromQuery] int frequency = 1, CancellationToken cancellationToken = default)
        {
            if (intervalType == DateIntervalType.OneTime)
            {
                return BadRequest();
            }

            var userId = User.FindFirst(HolefeederClaimTypes.HOLEFEEDER_ID)?.Value;

            var utc = effectiveDate.ToUniversalTime();

            var result = await _statisticsService.StatisticsByTagsAsync(userId, id, new DateTime(utc.Year, utc.Month, utc.Day), intervalType, frequency, cancellationToken).ConfigureAwait(false);

            return Ok(_mapper.Map<StatisticsDto<string>[]>(result));
        }
    }
}
