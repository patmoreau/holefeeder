import { tk } from '@holefeeder/shared/core';
import { Alert, Box, Skeleton, Stack, Typography } from '@mui/material';
import type { Theme } from '@mui/material/styles';
import type {} from '@mui/material/themeCssVarsAugmentation';
import { translate } from '@/shared/i18n/translate';
import { useDashboardSummary } from '../api/use-dashboard-summary';
import type { DashboardSummary } from '../domain/dashboard-summary';
import { currency, roundsToZero, type Tone, toneColor, toneOf, unsignedCurrency } from './tone';

const toneBackground: Record<Tone, string> = { positive: 'success.light', negative: 'error.light', neutral: 'background.paper' };

const toneChannel = (theme: Theme, tone: Tone) =>
  tone === 'positive' ? theme.vars!.palette.success.lightChannel : theme.vars!.palette.error.lightChannel;

const heroText = { fontSize: 15, color: 'primary.contrastText' };

const TrendBadge = ({ summary }: { summary: DashboardSummary }) => {
  const tone: Tone = summary.expenseVariation > 0 ? 'negative' : 'positive';
  return (
    <Stack
      direction="row"
      spacing={0.75}
      alignItems="center"
      data-testid="dashboard-spending-trend"
      sx={{
        py: 0.75,
        pl: 1.25,
        pr: 1.5,
        borderRadius: 3,
        border: 1,
        borderColor: toneBackground[tone],
        bgcolor: (theme) => `rgba(${toneChannel(theme, tone)} / 0.5)`,
        color: toneColor[tone],
      }}
    >
      <Box component="span" aria-hidden sx={{ fontSize: 14, fontWeight: 600 }}>
        {tone === 'negative' ? '↗' : '↘'}
      </Box>
      <Typography component="span" sx={{ fontSize: 17, color: 'inherit' }}>
        {translate(tk.dashboard.largeHeader.variation, { variation: currency(Math.abs(summary.expenseVariation)) })}
      </Typography>
    </Stack>
  );
};

const TotalPill = ({ label, value, testID }: { label: string; value: number; testID: string }) => {
  const tone = toneOf(value);
  return (
    <Stack spacing={0.75} alignItems="center">
      <Typography sx={heroText}>{label}</Typography>
      <Typography
        data-testid={testID}
        sx={{ px: 1.25, py: 0.5, borderRadius: 9999, fontSize: 17, fontWeight: 600, color: toneColor[tone], bgcolor: toneBackground[tone] }}
      >
        {unsignedCurrency(value)}
      </Typography>
    </Stack>
  );
};

const Hero = ({ summary }: { summary: DashboardSummary }) => (
  <Stack
    direction="row"
    alignItems="center"
    justifyContent="space-between"
    useFlexGap
    data-testid="dashboard-spending-hero"
    sx={{ p: 4, gap: 5, flexWrap: 'wrap', borderRadius: 4, bgcolor: 'primary.main' }}
  >
    <Stack spacing={1} alignItems="flex-start">
      <Typography sx={heroText}>{translate(tk.dashboard.largeHeader.spendingTitle)}</Typography>
      <Typography
        data-testid="dashboard-current-spending"
        sx={{ fontSize: 48, fontWeight: 700, lineHeight: 1.2, color: 'primary.contrastText' }}
      >
        {currency(summary.currentExpenses)}
      </Typography>
      {!roundsToZero(summary.averageExpenses) && <TrendBadge summary={summary} />}
    </Stack>
    <Stack direction="row" alignItems="center" spacing={5}>
      <Box sx={{ width: '1px', height: 72, bgcolor: (theme) => `rgba(${theme.vars!.palette.primary.contrastTextChannel} / 0.6)` }} />
      <TotalPill label={translate(tk.dashboard.largeHeader.netFlow)} value={summary.netFlow} testID="dashboard-net-flow" />
      <TotalPill label={translate(tk.accountCard.projected)} value={summary.projectedNetFlow} testID="dashboard-projected" />
    </Stack>
  </Stack>
);

export const SpendingHero = () => {
  const summary = useDashboardSummary();

  if (summary.isLoading) {
    return <Skeleton variant="rounded" height={189} data-testid="dashboard-spending-skeleton" sx={{ borderRadius: 4 }} />;
  }

  if (summary.isFailure) {
    return (
      <Alert severity="error" data-testid="dashboard-spending-error">
        {summary.errors.join(', ')}
      </Alert>
    );
  }

  return <Hero summary={summary.value} />;
};
