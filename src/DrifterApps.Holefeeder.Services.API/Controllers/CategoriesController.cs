using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.Business;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;
using DrifterApps.Holefeeder.Common.Authorization;
using DrifterApps.Holefeeder.Common.Enums;
using DrifterApps.Holefeeder.Common.Extensions;
using DrifterApps.Holefeeder.Services.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Services.API.Controllers
{
    [Route("api/v1/[controller]"), Authorize(Policy = Policies.RegisteredUser)]
    public class CategoriesController : Controller
    {
        private class Routes
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
        private readonly ILogger _logger;

        public CategoriesController(ICategoriesService service, IStatisticsService statisticsService, IMapper mapper, ILogger<CategoriesController> logger)
        {
            _service = service.ThrowIfNull(nameof(service));
            _statisticsService = statisticsService.ThrowIfNull(nameof(statisticsService));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
            _logger = logger.ThrowIfNull(nameof(logger));
        }

        [HttpGet(Name = Routes.GET_CATEGORIES)]
        [ProducesResponseType(typeof(CategoryDTO[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Get([FromQuery] int? offset, [FromQuery] int? limit, [FromQuery] string[] sort, [FromQuery] string[] filter)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;

            var result = await _service.FindAsync(userId, new QueryParams(offset, limit, sort, filter));
            return Ok(_mapper.Map<CategoryDTO[]>(result));
        }

        [HttpGet("{id}", Name = Routes.GET_CATEGORY)]
        [ProducesResponseType(typeof(CategoryDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Get(string id)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;
            if (!await _service.IsOwnerAsync(userId, id))
            {
                return NotFound();
            }

            var result = await _service.FindByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CategoryDTO>(result));
        }

        [HttpPost(Name = Routes.POST_CATEGORY)]
        [ProducesResponseType(typeof(CategoryDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Post([FromBody] CategoryDTO model)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.CreateAsync(userId, _mapper.Map<CategoryEntity>(model));

            return CreatedAtRoute(Routes.GET_CATEGORY, new { id = result.Id }, _mapper.Map<CategoryDTO>(result));
        }

        [HttpPut("{id}", Name = Routes.PUT_CATEGORY)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Put([FromRoute] string id, [FromBody] CategoryDTO model)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;
            if (!await _service.IsOwnerAsync(userId, id))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.UpdateAsync(id, _mapper.Map<CategoryEntity>(model));

            return NoContent();
        }

        [HttpDelete("{id}", Name = Routes.DELETE_CATEGORY)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;
            if (!await _service.IsOwnerAsync(userId, id))
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("statistics", Name = Routes.GET_STATISTICS)]
        [ProducesResponseType(typeof(StatisticsDTO<CategoryInfoDTO>[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetStatistics([FromQuery] DateTime effectiveDate, [FromQuery] DateIntervalType intervalType = DateIntervalType.Monthly, [FromQuery] int frequency = 1)
        {
            if (intervalType == DateIntervalType.OneTime)
            {
                return BadRequest();
            }

            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;

            var utc = effectiveDate.ToUniversalTime();

            var result = await _statisticsService.StatisticsAsync(userId, new DateTime(utc.Year, utc.Month, utc.Day), intervalType, frequency);

            return Ok(_mapper.Map<StatisticsDTO<CategoryInfoDTO>[]>(result));
        }

        [HttpGet("{id}/statistics", Name = Routes.GET_STATISTICS_BY_TAGS)]
        [ProducesResponseType(typeof(StatisticsDTO<string>[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetByTags([FromRoute] string id, [FromQuery] DateTime effectiveDate, [FromQuery] DateIntervalType intervalType = DateIntervalType.Monthly, [FromQuery] int frequency = 1)
        {
            if (intervalType == DateIntervalType.OneTime)
            {
                return BadRequest();
            }

            var userId = User.FindFirst(HolefeederClaimTypes.HolefeederId)?.Value;

            var utc = effectiveDate.ToUniversalTime();

            var result = await _statisticsService.StatisticsByTagsAsync(userId, id, new DateTime(utc.Year, utc.Month, utc.Day), intervalType, frequency);

            return Ok(_mapper.Map<StatisticsDTO<string>[]>(result));
        }
    }
}
