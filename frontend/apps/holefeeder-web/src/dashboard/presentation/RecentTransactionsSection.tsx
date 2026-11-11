import { tk } from '@holefeeder/shared/core';
import { Alert, Stack, Typography } from '@mui/material';
import { translate } from '@/shared/i18n/translate';
import type { Notify } from '@/shared/presentation/DashboardToast';
import { EmptyList } from '@/shared/presentation/EmptyList';
import { ListSkeleton } from '@/shared/presentation/ListSkeleton';
import { TransactionList } from '@/shared/presentation/TransactionList';
import { useRecentTransactions } from '../api/use-recent-transactions';

const TransactionsContent = ({ notify }: { notify: Notify }) => {
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

  return <TransactionList transactions={transactions.value} testID="dashboard-transaction" notify={notify} />;
};

export const RecentTransactionsSection = ({ notify }: { notify: Notify }) => (
  <Stack useFlexGap sx={{ gap: 1.5 }}>
    <Typography sx={{ fontSize: 15, color: 'text.secondary' }}>{translate(tk.recentTransactions.title)}</Typography>
    <TransactionsContent notify={notify} />
  </Stack>
);
