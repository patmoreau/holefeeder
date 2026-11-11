import { Paper } from '@mui/material';
import type { ReactNode } from 'react';
import { type Flow, FlowRow } from './FlowRow';

export type ListedTransaction = Flow & { id: string };

type TransactionListProps = { transactions: ListedTransaction[]; testID: string; children?: ReactNode };

export const TransactionList = ({ transactions, testID, children }: TransactionListProps) => (
  <Paper elevation={0} sx={{ px: 2, borderRadius: 3 }}>
    {transactions.map((transaction) => (
      <FlowRow key={transaction.id} flow={transaction} testID={testID} />
    ))}
    {children}
  </Paper>
);
