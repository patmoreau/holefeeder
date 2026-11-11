import { LocalFormatter, tk, today } from '@holefeeder/shared/core';
import LabelOutlinedIcon from '@mui/icons-material/LabelOutlined';
import { Alert, Box, Paper, Stack, Typography } from '@mui/material';
import { translate } from '@/shared/i18n/translate';
import { useRecentTransactions } from '../api/use-recent-transactions';
import type { DashboardTransaction } from '../domain/dashboard-transaction';
import { ListSkeleton } from './ListSkeleton';
import { currency } from './tone';

const lineHeight = 1.2;

const TagChip = ({ tag }: { tag: string }) => (
  <Box
    component="span"
    data-testid="dashboard-transaction-tag"
    sx={{
      px: 1,
      py: 0.25,
      borderRadius: 9999,
      fontSize: 13,
      lineHeight: lineHeight,
      color: 'primary.main',
      bgcolor: (theme) => `rgba(${theme.vars!.palette.primary.mainChannel} / 0.15)`,
    }}
  >
    {tag}
  </Box>
);

const TransactionRow = ({ transaction }: { transaction: DashboardTransaction }) => (
  <Stack
    direction="row"
    alignItems="center"
    spacing={1.5}
    data-testid="dashboard-transaction-row"
    sx={{ py: 1.5, '& + &': { borderTop: 1, borderColor: 'divider' } }}
  >
    <LabelOutlinedIcon data-testid="dashboard-transaction-icon" sx={{ fontSize: 20, color: transaction.categoryColor }} />
    <Stack spacing={0.5} sx={{ flexGrow: 1, minWidth: 0 }}>
      <Typography data-testid="dashboard-transaction-title" noWrap sx={{ fontSize: 17, fontWeight: 600, lineHeight: lineHeight }}>
        {transaction.title}
      </Typography>
      {(transaction.category || transaction.tags.length > 0) && (
        <Stack direction="row" alignItems="center" spacing={0.5} useFlexGap sx={{ flexWrap: 'wrap' }}>
          {transaction.category && (
            <Typography data-testid="dashboard-transaction-category" sx={{ fontSize: 13, lineHeight: lineHeight, color: 'text.secondary' }}>
              {transaction.category}
            </Typography>
          )}
          {transaction.tags.map((tag) => (
            <TagChip key={tag} tag={tag} />
          ))}
        </Stack>
      )}
    </Stack>
    <Stack spacing={0.25} alignItems="flex-end" sx={{ flexShrink: 0 }}>
      <Typography data-testid="dashboard-transaction-amount" sx={{ fontSize: 17, lineHeight: lineHeight }}>
        {currency(transaction.amount)}
      </Typography>
      <Typography data-testid="dashboard-transaction-date" sx={{ fontSize: 13, lineHeight: lineHeight, color: 'text.secondary' }}>
        {LocalFormatter.date(transaction.date, today(), 'en-CA', translate)}
      </Typography>
    </Stack>
  </Stack>
);

const EmptyTransactions = () => (
  <Paper elevation={0} data-testid="dashboard-transactions-empty" sx={{ p: 3, borderRadius: 3, textAlign: 'center' }}>
    <Typography sx={{ fontSize: 17, fontWeight: 600 }}>{translate(tk.emptyStates.transactions.title)}</Typography>
    <Typography sx={{ fontSize: 15, color: 'text.secondary' }}>{translate(tk.emptyStates.transactions.hint)}</Typography>
  </Paper>
);

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
    return <EmptyTransactions />;
  }

  return (
    <Paper elevation={0} sx={{ px: 2, borderRadius: 3 }}>
      {transactions.value.map((transaction) => (
        <TransactionRow key={transaction.id} transaction={transaction} />
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
