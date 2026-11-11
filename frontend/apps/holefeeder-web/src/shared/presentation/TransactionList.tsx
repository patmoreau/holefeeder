import { tk } from '@holefeeder/shared/core';
import { Paper } from '@mui/material';
import { type ReactNode, useState } from 'react';
import { useApiRefresh } from '@/shared/api/api-refresh';
import { useDeleteTransaction } from '@/shared/api/use-delete-transaction';
import { translate } from '@/shared/i18n/translate';
import type { Notify } from './DashboardToast';
import { type DeletedTransaction, DeleteTransactionDialog } from './DeleteTransactionDialog';
import { EditTransactionFlyout } from './EditTransactionFlyout';
import { type Flow, FlowRow } from './FlowRow';
import { TransactionActionsMenu } from './TransactionActionsMenu';

export type ListedTransaction = Flow & { id: string };

type TransactionListProps = { transactions: ListedTransaction[]; testID: string; notify: Notify; children?: ReactNode };

export const TransactionList = ({ transactions, testID, notify, children }: TransactionListProps) => {
  const deleteTransaction = useDeleteTransaction();
  const { refresh } = useApiRefresh();
  const [editing, setEditing] = useState<string | null>(null);
  const [deleting, setDeleting] = useState<DeletedTransaction | null>(null);

  const confirmDelete = async (transaction: DeletedTransaction) => {
    setDeleting(null);
    const result = await deleteTransaction(transaction.id);
    if (!result.isSuccess) {
      notify(translate(tk.editTransaction.deleteFailed, { title: transaction.title }));
      return;
    }
    notify(translate(tk.editTransaction.deleted, { title: transaction.title }));
    refresh();
  };

  return (
    <>
      <Paper elevation={0} sx={{ px: 2, borderRadius: 3 }}>
        {transactions.map((transaction) => (
          <FlowRow
            key={transaction.id}
            flow={transaction}
            testID={testID}
            onOpen={() => setEditing(transaction.id)}
            action={
              <TransactionActionsMenu
                testID={testID}
                onEdit={() => setEditing(transaction.id)}
                onDelete={() => setDeleting({ id: transaction.id, title: transaction.title })}
              />
            }
          />
        ))}
        {children}
      </Paper>
      <EditTransactionFlyout transactionId={editing} onClose={() => setEditing(null)} notify={notify} />
      {deleting && (
        <DeleteTransactionDialog transaction={deleting} onCancel={() => setDeleting(null)} onConfirm={() => void confirmDelete(deleting)} />
      )}
    </>
  );
};
