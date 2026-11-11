import { tk } from '@holefeeder/shared/core';
import { Alert, Button, Paper, Snackbar, SnackbarContent, Stack, Typography } from '@mui/material';
import { useState } from 'react';
import { useApiRefresh } from '@/shared/api/api-refresh';
import { translate } from '@/shared/i18n/translate';
import { useUpcomingActions } from '../api/use-upcoming-actions';
import { useUpcomingFlows } from '../api/use-upcoming-flows';
import type { DashboardUpcomingFlow } from '../domain/dashboard-upcoming-flow';
import { EmptyList } from './EmptyList';
import { FlowRow } from './FlowRow';
import { ListSkeleton } from './ListSkeleton';

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

const Toast = ({ message, onClose }: { message: string | null; onClose: () => void }) => (
  <Snackbar
    open={message !== null}
    autoHideDuration={4000}
    onClose={(_, reason) => reason !== 'clickaway' && onClose()}
    anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
  >
    <SnackbarContent data-testid="dashboard-toast" message={message} sx={{ borderRadius: 2.5, fontSize: 15 }} />
  </Snackbar>
);

type Notify = (message: string) => void;

const UpcomingList = ({ flows, notify }: { flows: DashboardUpcomingFlow[]; notify: Notify }) => {
  const actions = useUpcomingActions();
  const { refresh } = useApiRefresh();
  const [busy, setBusy] = useState(false);

  const run = async (action: Promise<{ isSuccess: boolean }>, success: string) => {
    setBusy(true);
    const result = await action;
    setBusy(false);
    notify(result.isSuccess ? success : translate(tk.payUpcoming.failed));
    if (result.isSuccess) refresh();
  };

  const pay = (flow: DashboardUpcomingFlow) => run(actions.pay(flow), translate(tk.payUpcoming.paid, { title: flow.title }));

  return (
    <Paper elevation={0} sx={{ px: 2, borderRadius: 3 }}>
      {flows.map((flow) => (
        <FlowRow
          key={flow.cashflowId + flow.date}
          flow={flow}
          testID="dashboard-upcoming"
          action={<PayButton disabled={busy} onClick={() => void pay(flow)} />}
        />
      ))}
    </Paper>
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
        {flows.errors.join(', ')}
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

export const UpcomingFlowsSection = () => {
  const [toast, setToast] = useState<string | null>(null);

  return (
    <Stack useFlexGap sx={{ gap: 1.5 }}>
      <Typography sx={{ fontSize: 15, color: 'text.secondary' }}>{translate(tk.upcomingList.title)}</Typography>
      <UpcomingContent notify={setToast} />
      <Toast message={toast} onClose={() => setToast(null)} />
    </Stack>
  );
};
