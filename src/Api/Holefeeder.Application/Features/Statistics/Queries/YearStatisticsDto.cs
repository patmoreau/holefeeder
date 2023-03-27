// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace Holefeeder.Application.Features.Statistics.Queries;

public record YearStatisticsDto(int Year, decimal Total, IEnumerable<MonthStatisticsDto> Months);
