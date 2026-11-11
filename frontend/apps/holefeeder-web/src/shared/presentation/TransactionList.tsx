import { Paper } from '@mui/material';
import { type ReactNode, useState } from 'react';
import type { Notify } from './DashboardToast';
import { EditTransactionFlyout } from './EditTransactionFlyout';
import { type Flow, FlowRow } from './FlowRow';

export type ListedTransaction = Flow & { id: string };

type TransactionListProps = { transactions: ListedTransaction[]; testID: string; notify: Notify; children?: ReactNode };

export const TransactionList = ({ transactions, testID, notify, children }: TransactionListProps) => {
  const [editing, setEditing] = useState<string | null>(null);

  return (
    <>
      <Paper elevation={0} sx={{ px: 2, borderRadius: 3 }}>
        {transactions.map((transaction) => (
          <FlowRow key={transaction.id} flow={transaction} testID={testID} onOpen={() => setEditing(transaction.id)} />
        ))}
        {children}
      </Paper>
      <EditTransactionFlyout transactionId={editing} onClose={() => setEditing(null)} notify={notify} />
    </>
  );
};
