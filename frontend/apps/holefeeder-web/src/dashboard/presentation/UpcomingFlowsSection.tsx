import { tk } from '@holefeeder/shared/core';
import { Alert, Button, Paper, Stack, Typography } from '@mui/material';
import { translate } from '@/shared/i18n/translate';
import { useUpcomingFlows } from '../api/use-upcoming-flows';
import { EmptyList } from './EmptyList';
import { FlowRow } from './FlowRow';
import { ListSkeleton } from './ListSkeleton';

const PayButton = () => (
  <Button
    variant="outlined"
    disabled
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

const UpcomingContent = () => {
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

  return (
    <Paper elevation={0} sx={{ px: 2, borderRadius: 3 }}>
      {flows.value.map((flow) => (
        <FlowRow key={flow.cashflowId + flow.date} flow={flow} testID="dashboard-upcoming" action={<PayButton />} />
      ))}
    </Paper>
  );
};

export const UpcomingFlowsSection = () => (
  <Stack useFlexGap sx={{ gap: 1.5 }}>
    <Typography sx={{ fontSize: 15, color: 'text.secondary' }}>{translate(tk.upcomingList.title)}</Typography>
    <UpcomingContent />
  </Stack>
);
