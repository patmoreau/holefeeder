import { tk } from '@holefeeder/shared/core';
import CloseIcon from '@mui/icons-material/Close';
import { Alert, Box, Drawer, IconButton, Stack, TextField, Typography } from '@mui/material';
import { useState } from 'react';
import { useApiRefresh } from '@/shared/api/api-refresh';
import { useModifyTransaction } from '@/shared/api/use-modify-transaction';
import { useNewTransactionOptions } from '@/shared/api/use-new-transaction-options';
import { useTransactionDetail } from '@/shared/api/use-transaction-detail';
import type { NewTransactionOptions } from '@/shared/domain/new-transaction-options';
import type { TransactionDetail } from '@/shared/domain/transaction-detail';
import { translate } from '@/shared/i18n/translate';
import { isAmountInput } from './amount-input';
import type { Notify } from './DashboardToast';
import { ListSkeleton } from './ListSkeleton';
import { TagsField } from './TagsField';
import { AccountSelect, AmountField, CategorySelect, type CategoryType, FlyoutFooter, TransactionTypeToggle } from './TransactionFields';

const testID = 'edit-transaction';

const withCurrent = <T extends { id: string }>(items: T[], current: T) =>
  items.some((item) => item.id === current.id) ? items : [...items, current];

type EditTransactionFormProps = { transaction: TransactionDetail; options: NewTransactionOptions; onClose: () => void; notify: Notify };

const EditTransactionForm = ({ transaction, options, onClose, notify }: EditTransactionFormProps) => {
  const modifyTransaction = useModifyTransaction();
  const { refresh } = useApiRefresh();
  const accounts = withCurrent(options.accounts, transaction.account);
  const allCategories = withCurrent(options.categories, transaction.category);
  const categoriesOf = (type: CategoryType) => allCategories.filter((category) => category.type === type);

  const [type, setType] = useState<CategoryType>(transaction.category.type);
  const [amount, setAmount] = useState(transaction.amount.toFixed(2));
  const [date, setDate] = useState(transaction.date);
  const [accountId, setAccountId] = useState(transaction.account.id);
  const [categoryId, setCategoryId] = useState(transaction.category.id);
  const [tags, setTags] = useState(transaction.tags);
  const [description, setDescription] = useState(transaction.description);
  const [failed, setFailed] = useState(false);
  const [busy, setBusy] = useState(false);

  const categories = categoriesOf(type);
  const valid = isAmountInput(amount) && date !== '' && categoryId !== '';
  const amountColor = type === 'expense' ? 'error.main' : 'success.main';

  const changeType = (next: CategoryType) => {
    setType(next);
    setCategoryId(categoriesOf(next)[0]?.id ?? '');
  };

  const save = async () => {
    setBusy(true);
    setFailed(false);
    const result = await modifyTransaction({
      id: transaction.id,
      date: date,
      amount: Number(amount),
      description: description,
      accountId: accountId,
      categoryId: categoryId,
      tags: tags,
    });
    setBusy(false);
    if (!result.isSuccess) {
      setFailed(true);
      return;
    }
    notify(translate(tk.newTransaction.saved));
    refresh();
    onClose();
  };

  return (
    <>
      <Stack spacing={2.5} sx={{ px: 3, pb: 3, flexGrow: 1, overflowY: 'auto' }}>
        <TransactionTypeToggle value={type} withTransfer={false} onChange={(next) => changeType(next as CategoryType)} />
        <AmountField testID={`${testID}-amount`} value={amount} color={amountColor} onChange={setAmount} />
        <TextField
          type="date"
          label={translate(tk.purchase.basicSection.date)}
          value={date}
          onChange={(event) => setDate(event.target.value)}
          slotProps={{ inputLabel: { shrink: true }, htmlInput: { 'data-testid': `${testID}-date` } }}
        />
        <AccountSelect
          label={translate(tk.purchase.basicSection.account)}
          testID={`${testID}-account`}
          accounts={accounts}
          value={accountId}
          onChange={setAccountId}
        />
        {categories.length === 0 ? (
          <Alert severity="info" data-testid={`${testID}-missing`}>
            {translate(tk.emptyStates.purchaseMissingCategory.title)}
          </Alert>
        ) : (
          <CategorySelect testID={`${testID}-category`} categories={categories} value={categoryId} onChange={setCategoryId} />
        )}
        {categoryId !== '' && <TagsField testID={testID} categoryId={categoryId} tags={tags} onChange={setTags} />}
        <TextField
          label={translate(tk.newTransaction.description)}
          placeholder={translate(tk.newTransaction.descriptionPlaceholder)}
          value={description}
          onChange={(event) => setDescription(event.target.value)}
          slotProps={{ inputLabel: { shrink: true }, htmlInput: { 'data-testid': `${testID}-description` } }}
        />
        {failed && (
          <Alert severity="error" data-testid={`${testID}-error`}>
            {translate(tk.editTransaction.failed)}
          </Alert>
        )}
      </Stack>
      <FlyoutFooter
        testID={testID}
        saveLabel={translate(tk.common.save)}
        disabled={busy || !valid}
        onSave={() => void save()}
        onCancel={onClose}
      />
    </>
  );
};

const EditTransactionContent = ({ transactionId, onClose, notify }: { transactionId: string; onClose: () => void; notify: Notify }) => {
  const transaction = useTransactionDetail(transactionId);
  const options = useNewTransactionOptions();

  if (transaction.isLoading || options.isLoading)
    return (
      <Box sx={{ px: 3 }}>
        <ListSkeleton testID={`${testID}-loading`} rows={4} />
      </Box>
    );

  if (transaction.isFailure || options.isFailure)
    return (
      <Box sx={{ px: 3 }}>
        <Alert severity="error" data-testid={`${testID}-load-error`}>
          {translate(tk.editTransaction.loadFailed)}
        </Alert>
      </Box>
    );

  return <EditTransactionForm transaction={transaction.value} options={options.value} onClose={onClose} notify={notify} />;
};

type EditTransactionFlyoutProps = { transactionId: string | null; onClose: () => void; notify: Notify };

export const EditTransactionFlyout = ({ transactionId, onClose, notify }: EditTransactionFlyoutProps) => (
  <Drawer
    anchor="right"
    open={transactionId !== null}
    onClose={onClose}
    slotProps={{ paper: { 'data-testid': testID, sx: { width: { xs: '100%', sm: 420 } } } as object }}
  >
    {transactionId !== null && (
      <Stack sx={{ height: '100%' }}>
        <Stack direction="row" alignItems="center" spacing={2} sx={{ px: 3, pt: 3, pb: 2 }}>
          <Typography component="h2" sx={{ flexGrow: 1, fontSize: 22, fontWeight: 700 }}>
            {translate(tk.editTransaction.title)}
          </Typography>
          <IconButton aria-label={translate(tk.newTransaction.close)} data-testid={`${testID}-close`} onClick={onClose}>
            <CloseIcon />
          </IconButton>
        </Stack>
        <EditTransactionContent key={transactionId} transactionId={transactionId} onClose={onClose} notify={notify} />
      </Stack>
    )}
  </Drawer>
);
