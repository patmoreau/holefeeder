import { tk } from '@holefeeder/shared/core';
import { Alert, Paper, Stack, Typography } from '@mui/material';
import { translate } from '@/shared/i18n/translate';
import { useRecentTransactions } from '../api/use-recent-transactions';
import { EmptyList } from './EmptyList';
import { FlowRow } from './FlowRow';
import { ListSkeleton } from './ListSkeleton';

const TransactionsContent = () => {
  const transactions = useRecentTransactions();

  if (transactions.isLoading) {
    return <ListSkeleton testID="dashboard-transactions-skeleton" rows={4} />;
  }

  if (transactions.isFailure) {
    return (
      <Alert severity="error" data-testid="dashboard-transactions-error">
        {transactions.errors.join(', ')}
      </Alert>
    );
  }

  if (transactions.value.length === 0) {
    return (
      <EmptyList
        testID="dashboard-transactions-empty"
        title={translate(tk.emptyStates.transactions.title)}
        hint={translate(tk.emptyStates.transactions.hint)}
      />
    );
  }

  return (
    <Paper elevation={0} sx={{ px: 2, borderRadius: 3 }}>
      {transactions.value.map((transaction) => (
        <FlowRow key={transaction.id} flow={transaction} testID="dashboard-transaction" />
      ))}
    </Paper>
  );
};

export const RecentTransactionsSection = () => (
  <Stack useFlexGap sx={{ gap: 1.5 }}>
    <Typography sx={{ fontSize: 15, color: 'text.secondary' }}>{translate(tk.recentTransactions.title)}</Typography>
    <TransactionsContent />
  </Stack>
);
