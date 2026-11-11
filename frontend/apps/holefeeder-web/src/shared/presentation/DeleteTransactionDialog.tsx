import { tk } from '@holefeeder/shared/core';
import { useTransactionDetail } from '@/shared/api/use-transaction-detail';
import { translate } from '@/shared/i18n/translate';
import { DeleteDialog } from './DeleteDialog';
import { mediumDate, unsignedCurrency } from './tone';

export type DeletedTransaction = { id: string; title: string };

type DeleteTransactionDialogProps = { transaction: DeletedTransaction; onCancel: () => void; onConfirm: () => void };

export const DeleteTransactionDialog = ({ transaction, onCancel, onConfirm }: DeleteTransactionDialogProps) => {
  const detail = useTransactionDetail(transaction.id);
  const message = detail.isSuccess
    ? translate(tk.editTransaction.deleteMessage, {
        amount: unsignedCurrency(detail.value.amount),
        title: transaction.title,
        date: mediumDate(detail.value.date),
        account: detail.value.account.name,
      })
    : translate(tk.editTransaction.deleteFallback, { title: transaction.title });

  return (
    <DeleteDialog
      open
      testID="transaction-delete"
      title={translate(tk.editTransaction.deleteTitle, { title: transaction.title })}
      message={message}
      onCancel={onCancel}
      onConfirm={onConfirm}
    />
  );
};
