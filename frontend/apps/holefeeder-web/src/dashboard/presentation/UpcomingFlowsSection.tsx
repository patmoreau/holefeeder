import { tk } from '@holefeeder/shared/core';
import { Alert, Button, Paper, Stack, Typography } from '@mui/material';
import { type ReactNode, useState } from 'react';
import { useApiRefresh } from '@/shared/api/api-refresh';
import { translate } from '@/shared/i18n/translate';
import type { Notify } from '@/shared/presentation/DashboardToast';
import { DeleteDialog } from '@/shared/presentation/DeleteDialog';
import { EmptyList } from '@/shared/presentation/EmptyList';
import { FlowRow } from '@/shared/presentation/FlowRow';
import { ListSkeleton } from '@/shared/presentation/ListSkeleton';
import { mediumDate } from '@/shared/presentation/tone';
import { type PayChanges, useUpcomingActions } from '../api/use-upcoming-actions';
import { useUpcomingFlows } from '../api/use-upcoming-flows';
import type { DashboardUpcomingFlow } from '../domain/dashboard-upcoming-flow';
import { PayFlyout } from './PayFlyout';
import { UpcomingActionsMenu } from './UpcomingActionsMenu';

const PayButton = ({ disabled, onClick }: { disabled: boolean; onClick: () => void }) => (
  <Button
    variant="outlined"
    disabled={disabled}
    onClick={onClick}
    data-testid="dashboard-upcoming-pay"
    sx={{
      flexShrink: 0,
      minWidth: 0,
      px: 1.75,
      py: 0.75,
      borderRadius: 2,
      fontSize: 15,
      fontWeight: 600,
      lineHeight: 1.2,
      textTransform: 'none',
      '&.Mui-disabled': { color: 'primary.main', borderColor: 'primary.main', opacity: 0.6 },
    }}
  >
    {translate(tk.swipeableActions.pay)}
  </Button>
);

const UpcomingList = ({ flows, notify }: { flows: DashboardUpcomingFlow[]; notify: Notify }) => {
  const actions = useUpcomingActions();
  const { refresh } = useApiRefresh();
  const [busy, setBusy] = useState(false);
  const [deleting, setDeleting] = useState<DashboardUpcomingFlow | null>(null);
  const [paying, setPaying] = useState<DashboardUpcomingFlow | null>(null);

  const run = async (action: Promise<{ isSuccess: boolean }>, success: string) => {
    setBusy(true);
    const result = await action;
    setBusy(false);
    notify(result.isSuccess ? success : translate(tk.payUpcoming.failed));
    if (result.isSuccess) refresh();
    return result.isSuccess;
  };

  const pay = (flow: DashboardUpcomingFlow, changes?: PayChanges) =>
    run(actions.pay(flow, changes), translate(tk.payUpcoming.paid, { title: flow.title }));

  const payWithChanges = async (flow: DashboardUpcomingFlow, changes: PayChanges) => {
    if (await pay(flow, changes)) setPaying(null);
  };

  const clear = (flow: DashboardUpcomingFlow) =>
    run(actions.clear(flow), translate(tk.payUpcoming.cleared, { title: flow.title, date: mediumDate(flow.date) }));

  const confirmDelete = () => {
    if (!deleting) return;
    const flow = deleting;
    setDeleting(null);
    void run(actions.cancel(flow), translate(tk.payUpcoming.deleted, { title: flow.title }));
  };

  const rowActions = (flow: DashboardUpcomingFlow): ReactNode => (
    <Stack direction="row" alignItems="center" spacing={0.5} sx={{ flexShrink: 0 }}>
      <PayButton disabled={busy} onClick={() => void pay(flow)} />
      <UpcomingActionsMenu
        disabled={busy}
        onPayWithChanges={() => setPaying(flow)}
        onClear={() => void clear(flow)}
        onDelete={() => setDeleting(flow)}
      />
    </Stack>
  );

  return (
    <>
      <Paper elevation={0} sx={{ px: 2, borderRadius: 3 }}>
        {flows.map((flow) => (
          <FlowRow key={flow.cashflowId + flow.date} flow={flow} testID="dashboard-upcoming" action={rowActions(flow)} />
        ))}
      </Paper>
      <PayFlyout flow={paying} busy={busy} onClose={() => setPaying(null)} onPay={(flow, changes) => void payWithChanges(flow, changes)} />
      <DeleteDialog
        open={deleting !== null}
        testID="dashboard-delete"
        title={translate(tk.payUpcoming.deleteTitle, { title: deleting?.title ?? '' })}
        message={translate(tk.payUpcoming.deleteMessage, { title: deleting?.title ?? '' })}
        onCancel={() => setDeleting(null)}
        onConfirm={confirmDelete}
      />
    </>
  );
};

const UpcomingContent = ({ notify }: { notify: Notify }) => {
  const flows = useUpcomingFlows();

  if (flows.isLoading) {
    return <ListSkeleton testID="dashboard-upcoming-skeleton" rows={3} />;
  }

  if (flows.isFailure) {
    return (
      <Alert severity="error" data-testid="dashboard-upcoming-error">
        {translate(tk.dashboard.upcomingLoadFailed)}
      </Alert>
    );
  }

  if (flows.value.length === 0) {
    return (
      <EmptyList
        testID="dashboard-upcoming-empty"
        title={translate(tk.emptyStates.upcoming.title)}
        hint={translate(tk.emptyStates.upcoming.hint)}
      />
    );
  }

  return <UpcomingList flows={flows.value} notify={notify} />;
};

export const UpcomingFlowsSection = ({ notify }: { notify: Notify }) => (
  <Stack useFlexGap sx={{ gap: 1.5 }}>
    <Typography sx={{ fontSize: 15, color: 'text.secondary' }}>{translate(tk.upcomingList.title)}</Typography>
    <UpcomingContent notify={notify} />
  </Stack>
);
