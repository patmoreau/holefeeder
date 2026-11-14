import { type PeriodSpan, AmountTone, type BandPoint, SurplusBands, tk } from '@holefeeder/shared/core';
import { Palettes } from '@holefeeder/shared/theme';
import { Box, Stack, ToggleButton, ToggleButtonGroup, Typography } from '@mui/material';
import { type KeyboardEvent, type PointerEvent, useEffect, useRef, useState } from 'react';
import { Language } from '@/shared/i18n/language';
import { translate } from '@/shared/i18n/translate';
import { currency, toneColor } from '@/shared/presentation/tone';
import type { Overview, Period, PeriodFlow, RangeOption } from '../domain/overview';

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
  '--chart-income': Palettes.light.chartIncome,
  '--chart-expenses': Palettes.light.chartExpenses,
  '--chart-deficit': Palettes.light.negative,
};

const darkColors = {
  '--chart-income': Palettes.dark.chartIncome,
  '--chart-expenses': Palettes.dark.chartExpenses,
  '--chart-deficit': Palettes.dark.error,
};

const daysIn = (period: Period) => (Date.parse(period.end) - Date.parse(period.start)) / 86_400_000 + 1;

const periodLabel = (period: Period) => {
  const days = daysIn(period);
  const options: Intl.DateTimeFormatOptions =
    days >= 360 ? { year: 'numeric' } : days >= 28 ? { month: 'short' } : { month: 'short', day: 'numeric' };
  return new Intl.DateTimeFormat(Language.locale(), { ...options, timeZone: 'UTC' }).format(new Date(period.start)).replace('.', '');
};

const longPeriodLabel = (period: Period) => {
  const days = daysIn(period);
  const options: Intl.DateTimeFormatOptions =
    days >= 360 ? { year: 'numeric' } : days >= 28 ? { month: 'long', year: 'numeric' } : { dateStyle: 'medium' };
  return new Intl.DateTimeFormat(Language.locale(), { ...options, timeZone: 'UTC' }).format(new Date(period.start));
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

const netText = (net: number) => {
  if (AmountTone.of(net) === 'positive') return translate(tk.insights.chart.tooltipSaved, { amount: currency(net) });
  if (AmountTone.of(net) === 'negative') return translate(tk.insights.net.overspent, { amount: currency(Math.abs(net)) });
  return translate(tk.insights.net.brokeEven);
};

const TooltipRow = ({
  label,
  value,
  stroke,
  dashed,
  testID,
}: {
  label: string;
  value: number;
  stroke: string;
  dashed?: boolean;
  testID: string;
}) => (
  <Stack direction="row" alignItems="center" spacing={1} data-testid={testID}>
    <svg width="12" height="8" aria-hidden>
      <line x1="0" y1="4" x2="12" y2="4" stroke={stroke} strokeWidth="2" strokeDasharray={dashed ? '3 2' : undefined} />
    </svg>
    <Typography component="span" sx={{ fontSize: 13, color: 'text.secondary', flexGrow: 1 }}>
      {label}
    </Typography>
    <Typography component="span" sx={{ fontSize: 13, fontWeight: 600 }}>
      {currency(value)}
    </Typography>
  </Stack>
);

const tooltipWidth = 200;
const tooltipGap = 12;

const Tooltip = ({ flow, left, width }: { flow: PeriodFlow; left: number; width: number }) => (
  <Stack
    data-testid="insights-chart-tooltip"
    aria-live="polite"
    spacing={0.5}
    sx={{
      position: 'absolute',
      top: padding.top,
      left: left + tooltipGap + tooltipWidth <= width ? left + tooltipGap : Math.max(left - tooltipGap - tooltipWidth, 0),
      width: tooltipWidth,
      p: 1.5,
      borderRadius: 2,
      bgcolor: 'background.default',
      boxShadow: 3,
      pointerEvents: 'none',
    }}
  >
    <Typography data-testid="insights-chart-tooltip-title" sx={{ fontSize: 13, fontWeight: 600 }}>
      {longPeriodLabel(flow.period)}
    </Typography>
    <TooltipRow
      label={translate(tk.insights.chart.income)}
      value={flow.income}
      stroke="var(--chart-income)"
      testID="insights-chart-tooltip-income"
    />
    <TooltipRow
      label={translate(tk.insights.chart.expenses)}
      value={flow.spending}
      stroke="var(--chart-expenses)"
      dashed
      testID="insights-chart-tooltip-expenses"
    />
    <Typography data-testid="insights-chart-tooltip-net" sx={{ fontSize: 13, fontWeight: 600, color: toneColor[AmountTone.of(flow.net)] }}>
      {netText(flow.net)}
    </Typography>
  </Stack>
);

const rangeLabel = (range: PeriodSpan) => translate(tk.insights.range[range.unit], { count: range.amount });

const RangeSwitch = ({ ranges, periods, onChange }: { ranges: RangeOption[]; periods: number; onChange: (periods: number) => void }) => (
  <ToggleButtonGroup
    exclusive
    size="small"
    value={periods}
    onChange={(_, value: number | null) => {
      if (value !== null) onChange(value);
    }}
    data-testid="insights-range"
    sx={{
      bgcolor: 'action.hover',
      borderRadius: 2,
      p: 0.25,
      '& .MuiToggleButton-root': { border: 0, borderRadius: 1.5, px: 1.5, textTransform: 'none', fontSize: 13 },
    }}
  >
    {ranges.map((option) => (
      <ToggleButton
        key={option.periods}
        value={option.periods}
        sx={{ '&.Mui-selected, &.Mui-selected:hover': { bgcolor: 'background.default', color: 'text.primary', fontWeight: 600 } }}
      >
        {rangeLabel(option.range)}
      </ToggleButton>
    ))}
  </ToggleButtonGroup>
);

const Plot = ({ series }: { series: PeriodFlow[] }) => {
  const { ref, width } = useWidth();
  const [selected, setSelected] = useState<number | null>(null);
  const last = series.length - 1;
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
  const indexAt = (event: PointerEvent<SVGRectElement>) => {
    const bounds = event.currentTarget.ownerSVGElement!.getBoundingClientRect();
    const position = ((event.clientX - bounds.left) * width) / bounds.width;
    return last < 1 ? 0 : Math.min(Math.max(Math.round(((position - padding.left) / plotWidth) * last), 0), last);
  };
  const keys: Record<string, (index: number) => number> = {
    ArrowLeft: (index) => Math.max(index - 1, 0),
    ArrowRight: (index) => Math.min(index + 1, last),
    Home: () => 0,
    End: () => last,
  };
  const onKeyDown = (event: KeyboardEvent) => {
    if (event.key === 'Escape') {
      setSelected(null);
      return;
    }
    const move = keys[event.key];
    if (!move) return;
    event.preventDefault();
    setSelected((index) => move(index ?? last));
  };
  const flow = selected === null ? undefined : series[selected];

  return (
    <Box
      ref={ref}
      tabIndex={0}
      data-testid="insights-chart-plot"
      onKeyDown={onKeyDown}
      onFocus={() => setSelected((index) => index ?? last)}
      onBlur={() => setSelected(null)}
      sx={{
        position: 'relative',
        width: '100%',
        minWidth: 0,
        overflow: 'hidden',
        outline: 'none',
        borderRadius: 1,
        '&:focus-visible': { boxShadow: (theme) => `0 0 0 2px ${theme.palette.primary.main}` },
      }}
    >
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
        {flow && selected !== null && (
          <g data-testid="insights-chart-marker" pointerEvents="none">
            <line x1={x(selected)} x2={x(selected)} y1={padding.top} y2={padding.top + plotHeight} stroke="currentColor" strokeOpacity="0.4" />
            <circle
              cx={x(selected)}
              cy={y(flow.income)}
              r="4"
              fill="var(--chart-income)"
              stroke="var(--mui-palette-background-paper)"
              strokeWidth="2"
            />
            <circle
              cx={x(selected)}
              cy={y(flow.spending)}
              r="4"
              fill="var(--chart-expenses)"
              stroke="var(--mui-palette-background-paper)"
              strokeWidth="2"
            />
          </g>
        )}
        <rect
          data-testid="insights-chart-hit"
          x={padding.left}
          y={padding.top}
          width={plotWidth}
          height={plotHeight}
          fill="transparent"
          onPointerDown={(event) => setSelected(indexAt(event))}
          onPointerMove={(event) => setSelected(indexAt(event))}
          onPointerLeave={(event) => {
            if (event.pointerType === 'mouse') setSelected(null);
          }}
        />
      </svg>
      {flow && selected !== null && <Tooltip flow={flow} left={x(selected)} width={width} />}
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

export const IncomeExpenseChart = ({
  overview,
  periods,
  onPeriodsChange,
}: {
  overview: Overview;
  periods: number;
  onPeriodsChange: (periods: number) => void;
}) => (
  <Stack
    spacing={2}
    data-testid="insights-chart"
    sx={(theme) => ({ py: 2.5, color: 'text.primary', ...colors, ...theme.applyStyles('dark', darkColors) })}
  >
    <Stack direction="row" alignItems="center" justifyContent="space-between" useFlexGap sx={{ gap: 1.5, flexWrap: 'wrap' }}>
      <Legend />
      <RangeSwitch ranges={overview.ranges} periods={periods} onChange={onPeriodsChange} />
    </Stack>
    <Plot series={overview.series} />
    <Stack direction="row" useFlexGap sx={{ gap: { xs: 2.5, md: 5 }, flexWrap: 'wrap' }}>
      <Total label={translate(tk.insights.chart.income)} value={overview.totalIncome} testID="insights-total-income" />
      <Total label={translate(tk.insights.chart.expenses)} value={overview.totalSpending} testID="insights-total-expenses" />
      <Total
        label={translate(tk.insights.chart.net)}
        value={overview.totalNet}
        testID="insights-total-net"
        color={toneColor[AmountTone.of(overview.totalNet)]}
      />
    </Stack>
    <Typography data-testid="insights-chart-footnote" sx={{ fontSize: 13, color: 'text.secondary' }}>
      {translate(tk.insights.chart.excludesTransfers)}
    </Typography>
    <Table series={overview.series} />
  </Stack>
);
