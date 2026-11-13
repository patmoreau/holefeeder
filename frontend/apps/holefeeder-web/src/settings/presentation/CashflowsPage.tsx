import { type CategoryType, type AsyncResult, tk } from '@holefeeder/shared/core';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import { Alert, Box, ButtonBase, IconButton, ListItem, Skeleton, Stack, Typography } from '@mui/material';
import { useState } from 'react';
import { Link } from 'react-router';
import { useApiRefresh } from '@/shared/api/api-refresh';
import { ApiRefreshProvider } from '@/shared/api/ApiRefreshProvider';
import { translate } from '@/shared/i18n/translate';
import { DashboardToast, type Notify } from '@/shared/presentation/DashboardToast';
import { DeleteDialog } from '@/shared/presentation/DeleteDialog';
import { EmptyList } from '@/shared/presentation/EmptyList';
import { RowActionsMenu } from '@/shared/presentation/RowActionsMenu';
import { Rows, Section } from '@/shared/presentation/Section';
import { currency } from '@/shared/presentation/tone';
import { useCancelCashflow } from '../api/use-cancel-cashflow';
import { useCashflows } from '../api/use-cashflows';
import type { Cashflow } from '../domain/cashflow';
import { scheduleOf } from './cashflow-schedule';
import { CashflowFlyout } from './CashflowFlyout';

const amountColor: Record<CategoryType, string> = { expense: 'error.main', gain: 'success.main' };

const secondaryText = { fontSize: 13, color: 'text.secondary' };

const CashflowDetails = ({ cashflow }: { cashflow: Cashflow }) => (
  <>
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
  </>
);

type CashflowActions = { onOpen: (cashflow: Cashflow) => void; onCancel: (cashflow: Cashflow) => void };

const CashflowRow = ({ cashflow, onOpen, onCancel }: { cashflow: Cashflow } & CashflowActions) => (
  <ListItem disablePadding data-testid="cashflows-row" sx={{ gap: 1, minWidth: 0 }}>
    <ButtonBase
      data-testid="cashflows-row-open"
      onClick={() => onOpen(cashflow)}
      sx={{
        display: 'flex',
        alignItems: 'center',
        gap: 1.5,
        py: 1.25,
        minWidth: 0,
        flexGrow: 1,
        justifyContent: 'flex-start',
        textAlign: 'left',
      }}
    >
      <CashflowDetails cashflow={cashflow} />
    </ButtonBase>
    <RowActionsMenu
      testID="cashflows-row"
      onEdit={() => onOpen(cashflow)}
      onDelete={() => onCancel(cashflow)}
      deleteLabel={translate(tk.cashflows.cancel)}
    />
  </ListItem>
);

type CashflowSectionProps = { title: string; testID: string; cashflows: Cashflow[] } & CashflowActions;

const CashflowSection = ({ title, testID, cashflows, ...actions }: CashflowSectionProps) =>
  cashflows.length === 0 ? null : (
    <Section title={title} testID={testID}>
      <Rows>
        {cashflows.map((cashflow) => (
          <CashflowRow key={cashflow.id} cashflow={cashflow} {...actions} />
        ))}
      </Rows>
    </Section>
  );

const CashflowList = ({ cashflows, ...actions }: { cashflows: AsyncResult<Cashflow[]> } & CashflowActions) => {
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
      <CashflowSection title={translate(tk.categories.expenses)} testID="cashflows-expenses" cashflows={ofType('expense')} {...actions} />
      <CashflowSection title={translate(tk.categories.income)} testID="cashflows-income" cashflows={ofType('gain')} {...actions} />
    </Stack>
  );
};

const CashflowsContent = ({ notify }: { notify: Notify }) => {
  const cashflows = useCashflows();
  const [editing, setEditing] = useState<Cashflow | null>(null);
  const [cancelling, setCancelling] = useState<Cashflow | null>(null);
  const cancelCashflow = useCancelCashflow();
  const { refresh } = useApiRefresh();

  const confirmCancel = async (cashflow: Cashflow) => {
    setCancelling(null);
    const result = await cancelCashflow(cashflow.id);
    notify(translate(result.isSuccess ? tk.cashflows.cancelled : tk.cashflows.cancelFailed, { name: cashflow.title }));
    if (result.isSuccess) refresh();
  };

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
      <CashflowList cashflows={cashflows} onOpen={setEditing} onCancel={setCancelling} />
      <CashflowFlyout cashflow={editing} onClose={() => setEditing(null)} notify={notify} />
      {cancelling !== null && (
        <DeleteDialog
          open
          testID="cashflow-cancel-dialog"
          title={translate(tk.cashflows.cancelTitle, { name: cancelling.title })}
          message={translate(tk.cashflows.cancelMessage, { name: cancelling.title })}
          cancelLabel={translate(tk.cashflows.keep)}
          confirmLabel={translate(tk.cashflows.cancel)}
          onCancel={() => setCancelling(null)}
          onConfirm={() => void confirmCancel(cancelling)}
        />
      )}
    </Stack>
  );
};

export const CashflowsPage = () => {
  const [toast, setToast] = useState<string | null>(null);

  return (
    <ApiRefreshProvider>
      <CashflowsContent notify={setToast} />
      <DashboardToast message={toast} onClose={() => setToast(null)} />
    </ApiRefreshProvider>
  );
};
