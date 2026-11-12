import { type AsyncResult, tk } from '@holefeeder/shared/core';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import { Alert, Box, IconButton, ListItem, Skeleton, Stack, Typography } from '@mui/material';
import { Link } from 'react-router';
import { translate } from '@/shared/i18n/translate';
import { EmptyList } from '@/shared/presentation/EmptyList';
import { currency, mediumDate } from '@/shared/presentation/tone';
import { useCashflows } from '../api/use-cashflows';
import type { Cashflow } from '../domain/cashflow';
import type { CategoryType } from '../domain/category';
import { periodLabel } from './budget-summary';
import { Rows, Section } from './SettingsList';

const scheduleOf = (cashflow: Cashflow) => {
  const period = periodLabel(cashflow.intervalType, cashflow.frequency);
  return cashflow.nextDate === null ? period : translate(tk.cashflows.next, { period: period, date: mediumDate(cashflow.nextDate) });
};

const amountColor: Record<CategoryType, string> = { expense: 'error.main', gain: 'success.main' };

const secondaryText = { fontSize: 13, color: 'text.secondary' };

const CashflowRow = ({ cashflow }: { cashflow: Cashflow }) => (
  <ListItem disablePadding data-testid="cashflows-row" sx={{ gap: 1.5, py: 1.25, minWidth: 0 }}>
    <Box aria-hidden sx={{ width: 12, height: 12, borderRadius: '50%', bgcolor: cashflow.categoryColor, flexShrink: 0 }} />
    <Stack sx={{ minWidth: 0, flexGrow: 1 }}>
      <Typography data-testid="cashflows-row-title" noWrap sx={{ fontSize: 17 }}>
        {cashflow.title}
      </Typography>
      <Typography data-testid="cashflows-row-details" noWrap sx={secondaryText}>
        {`${cashflow.categoryName} · ${cashflow.accountName}`}
      </Typography>
    </Stack>
    <Stack alignItems="flex-end" sx={{ minWidth: 0, flexShrink: 1, textAlign: 'right' }}>
      <Typography data-testid="cashflows-row-amount" sx={{ fontSize: 15, fontWeight: 600, color: amountColor[cashflow.categoryType] }}>
        {currency(cashflow.amount)}
      </Typography>
      <Typography data-testid="cashflows-row-schedule" sx={secondaryText}>
        {scheduleOf(cashflow)}
      </Typography>
    </Stack>
  </ListItem>
);

type CashflowSectionProps = { title: string; testID: string; cashflows: Cashflow[] };

const CashflowSection = ({ title, testID, cashflows }: CashflowSectionProps) =>
  cashflows.length === 0 ? null : (
    <Section title={title} testID={testID}>
      <Rows>
        {cashflows.map((cashflow) => (
          <CashflowRow key={cashflow.id} cashflow={cashflow} />
        ))}
      </Rows>
    </Section>
  );

const CashflowList = ({ cashflows }: { cashflows: AsyncResult<Cashflow[]> }) => {
  if (cashflows.isLoading) return <Skeleton variant="rounded" height={240} data-testid="cashflows-loading" sx={{ borderRadius: 3 }} />;

  if (cashflows.isFailure)
    return (
      <Alert severity="error" data-testid="cashflows-load-error">
        {translate(tk.cashflows.loadFailed)}
      </Alert>
    );

  const ofType = (type: CategoryType) => cashflows.value.filter((cashflow) => cashflow.categoryType === type);

  return (
    <Stack spacing={3.5}>
      {cashflows.value.length === 0 && (
        <EmptyList testID="cashflows-empty" title={translate(tk.cashflows.emptyTitle)} hint={translate(tk.cashflows.emptyHint)} />
      )}
      <CashflowSection title={translate(tk.categories.expenses)} testID="cashflows-expenses" cashflows={ofType('expense')} />
      <CashflowSection title={translate(tk.categories.income)} testID="cashflows-income" cashflows={ofType('gain')} />
    </Stack>
  );
};

export const CashflowsPage = () => {
  const cashflows = useCashflows();

  return (
    <Stack spacing={3.5} data-testid="cashflows-page" sx={{ maxWidth: 720 }}>
      <Stack direction="row" alignItems="center" spacing={1.5}>
        <IconButton
          component={Link}
          to="/settings"
          data-testid="cashflows-back"
          aria-label={translate(tk.cashflows.back)}
          sx={{ bgcolor: 'background.paper', color: 'text.primary', flexShrink: 0 }}
        >
          <ChevronLeftIcon />
        </IconButton>
        <Typography variant="h1" noWrap sx={{ flexGrow: 1, minWidth: 0, fontSize: 28, fontWeight: 700 }}>
          {translate(tk.cashflowsSection.title)}
        </Typography>
      </Stack>
      <CashflowList cashflows={cashflows} />
    </Stack>
  );
};
