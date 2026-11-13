import { LocalFormatter, type NetBalanceKind, AmountTone, tk } from '@holefeeder/shared/core';
import { Palettes } from '@holefeeder/shared/theme';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import NorthEastIcon from '@mui/icons-material/NorthEast';
import SouthEastIcon from '@mui/icons-material/SouthEast';
import { Alert, Box, ListItemButton, Skeleton, Stack, Typography } from '@mui/material';
import type { ReactNode } from 'react';
import { Link } from 'react-router';
import { Language } from '@/shared/i18n/language';
import { translate } from '@/shared/i18n/translate';
import { Rows, Section } from '@/shared/presentation/Section';
import { currency, toneColor, unsignedCurrency } from '@/shared/presentation/tone';
import { useOverview } from '../api/use-overview';
import type { Overview } from '../domain/overview';
import { IncomeExpenseChart } from './IncomeExpenseChart';
import { useInsightsRange } from './use-insights-range';

const heroText = { fontSize: 15, color: 'primary.contrastText' };

const outcomeTone: Record<NetBalanceKind, AmountTone> = { none: 'neutral', saved: 'positive', overspent: 'negative', brokeEven: 'neutral' };

const outcomeText = (overview: Overview) => {
  switch (overview.outcome) {
    case 'saved':
      return translate(tk.insights.net.saved, { percentage: LocalFormatter.percent(overview.savedPercentage, Language.locale()) });
    case 'overspent':
      return translate(tk.insights.net.overspent, { amount: unsignedCurrency(overview.net) });
    default:
      return translate(tk.insights.net.brokeEven);
  }
};

const OutcomeBadge = ({ overview }: { overview: Overview }) =>
  overview.outcome === 'none' ? null : (
    <Typography
      data-testid="insights-outcome"
      sx={{
        py: 0.25,
        px: 1.25,
        borderRadius: 9999,
        bgcolor: 'background.paper',
        fontSize: 14,
        fontWeight: 700,
        color: toneColor[outcomeTone[overview.outcome]],
      }}
    >
      {outcomeText(overview)}
    </Typography>
  );

const Total = ({ label, value, testID }: { label: string; value: number; testID: string }) => (
  <Stack spacing={0.5} sx={{ minWidth: 0 }}>
    <Typography sx={heroText}>{label}</Typography>
    <Typography data-testid={testID} noWrap sx={{ fontSize: 17, fontWeight: 600, color: 'primary.contrastText' }}>
      {currency(value)}
    </Typography>
  </Stack>
);

const Hero = ({ overview }: { overview: Overview }) => (
  <Stack
    direction={{ xs: 'column', md: 'row' }}
    alignItems={{ xs: 'stretch', md: 'center' }}
    justifyContent="space-between"
    useFlexGap
    data-testid="insights-hero"
    sx={{ p: { xs: 3, md: 4 }, gap: { xs: 2.5, md: 5 }, borderRadius: 4, bgcolor: 'primary.main', minWidth: 0 }}
  >
    <Stack spacing={1} alignItems="flex-start" sx={{ minWidth: 0 }}>
      <Typography sx={heroText}>{translate(tk.insights.net.title)}</Typography>
      <Typography
        data-testid="insights-net"
        sx={{ fontSize: { xs: 40, md: 48 }, fontWeight: 700, lineHeight: 1.2, color: 'primary.contrastText', overflowWrap: 'anywhere' }}
      >
        {unsignedCurrency(overview.net)}
      </Typography>
      <OutcomeBadge overview={overview} />
    </Stack>
    <Stack direction="row" useFlexGap sx={{ gap: 5, minWidth: 0 }}>
      <Total label={translate(tk.insights.chart.income)} value={overview.income} testID="insights-income" />
      <Total label={translate(tk.insights.spending.title)} value={overview.spending} testID="insights-spending" />
    </Stack>
  </Stack>
);

const LinkRow = ({
  to,
  label,
  value,
  tile,
  icon,
  testID,
}: {
  to: string;
  label: string;
  value: number;
  tile: string;
  icon: ReactNode;
  testID: string;
}) => (
  <ListItemButton component={Link} to={to} data-testid={testID} disableGutters sx={{ py: 1.5, gap: 1.5, minWidth: 0, borderRadius: 2 }}>
    <Box
      aria-hidden
      sx={{
        width: 28,
        height: 28,
        borderRadius: 1.5,
        bgcolor: tile,
        color: 'common.white',
        display: 'grid',
        placeItems: 'center',
        flexShrink: 0,
        '& svg': { fontSize: 18 },
      }}
    >
      {icon}
    </Box>
    <Typography noWrap sx={{ fontSize: 17, flexGrow: 1, minWidth: 0 }}>
      {label}
    </Typography>
    <Typography noWrap sx={{ fontSize: 17, color: 'text.secondary', flexShrink: 0 }}>
      {currency(value)}
    </Typography>
    <ChevronRightIcon sx={{ color: 'text.disabled', flexShrink: 0 }} />
  </ListItemButton>
);

const Content = () => {
  const { periods, choose } = useInsightsRange();
  const overview = useOverview(periods);

  if (overview.isLoading) return <Skeleton variant="rounded" height={189} data-testid="insights-loading" sx={{ borderRadius: 4 }} />;

  if (overview.isFailure)
    return (
      <Alert severity="error" data-testid="insights-load-error">
        {translate(tk.insights.loadFailed)}
      </Alert>
    );

  return (
    <>
      <Hero overview={overview.value} />
      <Section title={translate(tk.insights.chart.title)} testID="insights-chart-section">
        <IncomeExpenseChart overview={overview.value} periods={periods} onPeriodsChange={choose} />
      </Section>
      <Section title={translate(tk.insights.thisPeriod)} testID="insights-this-period">
        <Rows>
          <LinkRow
            to="/insights/income"
            label={translate(tk.insights.income.link)}
            value={overview.value.income}
            tile="success.main"
            icon={<NorthEastIcon />}
            testID="insights-income-link"
          />
          <LinkRow
            to="/insights/spending"
            label={translate(tk.insights.spending.link)}
            value={overview.value.spending}
            tile={Palettes.light.chartExpenses}
            icon={<SouthEastIcon />}
            testID="insights-spending-link"
          />
        </Rows>
      </Section>
    </>
  );
};

export const InsightsPage = () => (
  <Stack spacing={3.5} data-testid="insights-page" sx={{ minWidth: 0 }}>
    <Typography variant="h1" noWrap sx={{ fontSize: 28, fontWeight: 700 }}>
      {translate(tk.insights.title)}
    </Typography>
    <Content />
  </Stack>
);
