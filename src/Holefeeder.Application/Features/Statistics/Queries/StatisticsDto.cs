// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace Holefeeder.Application.Features.Statistics.Queries;

public record StatisticsDto(Guid CategoryId, string Category, string Color, decimal MonthlyAverage, IEnumerable<YearStatisticsDto> Years);
