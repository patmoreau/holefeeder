import { type BandPoint, SurplusBands, tk } from '@holefeeder/shared/core';
import { Box, Stack, Typography } from '@mui/material';
import { useEffect, useRef, useState } from 'react';
import { Language } from '@/shared/i18n/language';
import { translate } from '@/shared/i18n/translate';
import { currency, toneColor, toneOf } from '@/shared/presentation/tone';
import type { Overview, Period, PeriodFlow } from '../domain/overview';

const visuallyHidden = {
  position: 'absolute',
  width: '1px',
  height: '1px',
  overflow: 'hidden',
  clip: 'rect(0 0 0 0)',
  whiteSpace: 'nowrap',
  border: 0,
  padding: 0,
  margin: '-1px',
} as const;

const height = 260;
const padding = { top: 8, right: 12, bottom: 28, left: 48 };

const colors = {
  '--chart-income': '#2E7D32',
  '--chart-expenses': '#FF8C42',
  '--chart-deficit': '#D32F2F',
};

const darkColors = {
  '--chart-income': '#26A69A',
  '--chart-expenses': '#D96F26',
  '--chart-deficit': '#FF453A',
};

const daysIn = (period: Period) => (Date.parse(period.end) - Date.parse(period.start)) / 86_400_000 + 1;

const periodLabel = (period: Period) => {
  const days = daysIn(period);
  const options: Intl.DateTimeFormatOptions =
    days >= 360 ? { year: 'numeric' } : days >= 28 ? { month: 'short' } : { month: 'short', day: 'numeric' };
  return new Intl.DateTimeFormat(Language.locale(), { ...options, timeZone: 'UTC' }).format(new Date(period.start)).replace('.', '');
};

const compactCurrency = (value: number) =>
  new Intl.NumberFormat(Language.locale(), { style: 'currency', currency: 'CAD', notation: 'compact', maximumFractionDigits: 1 }).format(value);

const niceStep = (max: number) => {
  const rough = max / 3;
  const magnitude = 10 ** Math.floor(Math.log10(rough));
  const normalized = rough / magnitude;
  return (normalized <= 1 ? 1 : normalized <= 2 ? 2 : normalized <= 5 ? 5 : 10) * magnitude;
};

const ticksFor = (series: PeriodFlow[]) => {
  const max = Math.max(0, ...series.flatMap((flow) => [flow.income, flow.spending]));
  if (max === 0) return [0, 1];
  const step = niceStep(max);
  return Array.from({ length: Math.ceil(max / step) + 1 }, (_, index) => index * step);
};

const useWidth = () => {
  const ref = useRef<HTMLDivElement>(null);
  const [width, setWidth] = useState(600);

  useEffect(() => {
    const element = ref.current;
    if (!element) return;
    const observer = new ResizeObserver(([entry]) => setWidth(entry.contentRect.width));
    observer.observe(element);
    return () => observer.disconnect();
  }, []);

  return { ref: ref, width: width };
};

const round = (value: number) => Math.round(value * 10) / 10;

const Legend = () => (
  <Stack direction="row" useFlexGap sx={{ gap: 2.5, flexWrap: 'wrap' }}>
    {[
      { label: tk.insights.chart.income, mark: <line x1="0" y1="6" x2="16" y2="6" stroke="var(--chart-income)" strokeWidth="2" /> },
      {
        label: tk.insights.chart.expenses,
        mark: <line x1="0" y1="6" x2="16" y2="6" stroke="var(--chart-expenses)" strokeWidth="2" strokeDasharray="4 3" />,
      },
      {
        label: tk.insights.chart.deficit,
        mark: <rect x="2" y="0" width="12" height="12" rx="2" fill="var(--chart-deficit)" fillOpacity="0.16" />,
      },
    ].map(({ label, mark }) => (
      <Stack key={label} direction="row" alignItems="center" spacing={0.75}>
        <svg width="16" height="12" aria-hidden>
          {mark}
        </svg>
        <Typography data-testid="insights-chart-legend" sx={{ fontSize: 13, color: 'text.secondary' }}>
          {translate(label)}
        </Typography>
      </Stack>
    ))}
  </Stack>
);

const Plot = ({ series }: { series: PeriodFlow[] }) => {
  const { ref, width } = useWidth();
  const ticks = ticksFor(series);
  const top = ticks[ticks.length - 1];
  const plotWidth = Math.max(0, width - padding.left - padding.right);
  const plotHeight = height - padding.top - padding.bottom;
  const x = (index: number) => round(padding.left + (series.length > 1 ? (index / (series.length - 1)) * plotWidth : plotWidth / 2));
  const y = (value: number) => round(padding.top + plotHeight - (value / top) * plotHeight);
  const line = (values: number[]) => values.map((value, index) => `${index === 0 ? 'M' : 'L'}${x(index)},${y(value)}`).join('');
  const band = (points: BandPoint[]) =>
    [...points.map((point) => `${x(point.x)},${y(point.high)}`), ...[...points].reverse().map((point) => `${x(point.x)},${y(point.low)}`)].join(
      ' '
    );
  const bands = SurplusBands.of(series.map((flow) => ({ income: flow.income, expenses: flow.spending })));

  return (
    <Box ref={ref} sx={{ width: '100%', minWidth: 0, overflow: 'hidden' }}>
      <svg
        viewBox={`0 0 ${width} ${height}`}
        height={height}
        role="img"
        aria-label={translate(tk.insights.chart.title)}
        style={{ display: 'block', width: '100%' }}
      >
        {ticks.map((tick) => (
          <g key={tick}>
            <line x1={padding.left} x2={width - padding.right} y1={y(tick)} y2={y(tick)} stroke="currentColor" strokeOpacity="0.12" />
            <text
              data-testid="insights-chart-y-label"
              x={padding.left - 8}
              y={y(tick)}
              textAnchor="end"
              dominantBaseline="middle"
              fontSize="12"
              fill="currentColor"
              fillOpacity="0.6"
            >
              {compactCurrency(tick)}
            </text>
          </g>
        ))}
        {bands.map((surplusBand, index) => (
          <polygon
            key={index}
            data-testid="insights-chart-band"
            data-kind={surplusBand.kind}
            points={band(surplusBand.points)}
            fill={surplusBand.kind === 'surplus' ? 'var(--chart-income)' : 'var(--chart-deficit)'}
            fillOpacity="0.16"
          />
        ))}
        <path
          data-testid="insights-chart-income"
          d={line(series.map((flow) => flow.income))}
          fill="none"
          stroke="var(--chart-income)"
          strokeWidth="2"
          strokeLinejoin="round"
        />
        <path
          data-testid="insights-chart-expenses"
          d={line(series.map((flow) => flow.spending))}
          fill="none"
          stroke="var(--chart-expenses)"
          strokeWidth="2"
          strokeDasharray="6 4"
          strokeLinejoin="round"
        />
        {series.map((flow, index) => (
          <text
            key={flow.period.start}
            data-testid="insights-chart-x-label"
            x={x(index)}
            y={height - 8}
            textAnchor={index === 0 && series.length > 1 ? 'start' : index === series.length - 1 && series.length > 1 ? 'end' : 'middle'}
            fontSize="12"
            fill="currentColor"
            fillOpacity="0.6"
          >
            {periodLabel(flow.period)}
          </text>
        ))}
      </svg>
    </Box>
  );
};

const Total = ({ label, value, testID, color }: { label: string; value: number; testID: string; color?: string }) => (
  <Stack spacing={0.25} sx={{ minWidth: 0 }}>
    <Typography sx={{ fontSize: 13, color: 'text.secondary' }}>{label}</Typography>
    <Typography data-testid={testID} noWrap sx={{ fontSize: 17, fontWeight: 600, color: color ?? 'text.primary' }}>
      {currency(value)}
    </Typography>
  </Stack>
);

const Table = ({ series }: { series: PeriodFlow[] }) => (
  <Box component="table" data-testid="insights-chart-table" aria-label={translate(tk.insights.chart.title)} sx={visuallyHidden}>
    <thead>
      <tr>
        <th scope="col" />
        <th scope="col">{translate(tk.insights.chart.income)}</th>
        <th scope="col">{translate(tk.insights.chart.expenses)}</th>
      </tr>
    </thead>
    <tbody>
      {series.map((flow) => (
        <tr key={flow.period.start}>
          <th scope="row">{periodLabel(flow.period)}</th>
          <td>{currency(flow.income)}</td>
          <td>{currency(flow.spending)}</td>
        </tr>
      ))}
    </tbody>
  </Box>
);

export const IncomeExpenseChart = ({ overview }: { overview: Overview }) => (
  <Stack
    spacing={2}
    data-testid="insights-chart"
    sx={(theme) => ({ py: 2.5, color: 'text.primary', ...colors, ...theme.applyStyles('dark', darkColors) })}
  >
    <Legend />
    <Plot series={overview.series} />
    <Stack direction="row" useFlexGap sx={{ gap: { xs: 2.5, md: 5 }, flexWrap: 'wrap' }}>
      <Total label={translate(tk.insights.chart.income)} value={overview.totalIncome} testID="insights-total-income" />
      <Total label={translate(tk.insights.chart.expenses)} value={overview.totalSpending} testID="insights-total-expenses" />
      <Total
        label={translate(tk.insights.chart.net)}
        value={overview.totalNet}
        testID="insights-total-net"
        color={toneColor[toneOf(overview.totalNet)]}
      />
    </Stack>
    <Typography data-testid="insights-chart-footnote" sx={{ fontSize: 13, color: 'text.secondary' }}>
      {translate(tk.insights.chart.excludesTransfers)}
    </Typography>
    <Table series={overview.series} />
  </Stack>
);
