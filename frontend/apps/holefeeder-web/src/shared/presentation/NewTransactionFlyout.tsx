import { tk } from '@holefeeder/shared/core';
import CloseIcon from '@mui/icons-material/Close';
import {
  Alert,
  Box,
  Button,
  Divider,
  Drawer,
  IconButton,
  InputBase,
  MenuItem,
  Stack,
  TextField,
  ToggleButton,
  ToggleButtonGroup,
  Typography,
} from '@mui/material';
import { useState } from 'react';
import { useApiRefresh } from '@/shared/api/api-refresh';
import { useNewTransactionActions } from '@/shared/api/use-new-transaction-actions';
import { useNewTransactionOptions } from '@/shared/api/use-new-transaction-options';
import type { NewTransactionOptions, TransactionAccountOption, TransactionCategoryOption } from '@/shared/domain/new-transaction-options';
import { translate } from '@/shared/i18n/translate';
import { isAmountInput } from './amount-input';
import type { Notify } from './DashboardToast';
import { EmptyList } from './EmptyList';
import { ListSkeleton } from './ListSkeleton';
import { isRepeatDraftValid, type RepeatDraft, RepeatField } from './RepeatField';
import { TagsField } from './TagsField';

const footerButton = { py: 1.25, borderRadius: 2.5, fontSize: 15, fontWeight: 600, textTransform: 'none' };

type CategoryType = TransactionCategoryOption['type'];

type TransactionType = CategoryType | 'transfer';

const today = () => new Date().toLocaleDateString('en-CA');

const typeButton = { textTransform: 'none', fontSize: 15 };

type AccountSelectProps = {
  label: string;
  testID: string;
  accounts: TransactionAccountOption[];
  value: string;
  onChange: (id: string) => void;
};

const AccountSelect = ({ label, testID, accounts, value, onChange }: AccountSelectProps) => (
  <TextField
    select
    label={label}
    value={value}
    onChange={(event) => onChange(event.target.value)}
    slotProps={{ select: { SelectDisplayProps: { 'data-testid': testID } as object } }}
  >
    {accounts.map((account) => (
      <MenuItem key={account.id} value={account.id}>
        {account.name}
      </MenuItem>
    ))}
  </TextField>
);

type NewTransactionFormProps = { options: NewTransactionOptions; accountId?: string; onClose: () => void; notify: Notify };

const NewTransactionForm = ({ options, accountId: preselectedAccountId, onClose, notify }: NewTransactionFormProps) => {
  const actions = useNewTransactionActions();
  const { refresh } = useApiRefresh();
  const categoriesOf = (type: CategoryType) => options.categories.filter((category) => category.type === type);

  const [type, setType] = useState<TransactionType>('expense');
  const [amount, setAmount] = useState('0.00');
  const [date, setDate] = useState(today);
  const preselected = options.accounts.find((account) => account.id === preselectedAccountId);
  const initialAccountId = preselected?.id ?? options.accounts[0]?.id ?? '';
  const [accountId, setAccountId] = useState(initialAccountId);
  const [toAccountId, setToAccountId] = useState(options.accounts.find((account) => account.id !== initialAccountId)?.id ?? initialAccountId);
  const [categoryId, setCategoryId] = useState(categoriesOf('expense')[0]?.id ?? '');
  const [tags, setTags] = useState<string[]>([]);
  const [description, setDescription] = useState('');
  const [repeat, setRepeat] = useState<RepeatDraft | null>(null);
  const [busy, setBusy] = useState(false);

  const transfer = type === 'transfer';
  const categories = transfer ? [] : categoriesOf(type);
  const validAmount = isAmountInput(amount);
  const validRepeat = repeat === null || isRepeatDraftValid(repeat);
  const sameAccount = transfer && accountId === toAccountId;
  const valid = validAmount && date !== '' && (transfer ? !sameAccount : validRepeat && categoryId !== '');
  const amountColor = { expense: 'error.main', gain: 'success.main', transfer: 'primary.main' }[type];
  const accountName = (id: string) => options.accounts.find((account) => account.id === id)?.name ?? '';

  const changeType = (next: TransactionType | null) => {
    if (next === null) return;
    setType(next);
    if (next !== 'transfer') setCategoryId(categoriesOf(next)[0]?.id ?? '');
  };

  const record = () =>
    transfer
      ? actions.recordTransfer({
          date: date,
          amount: Number(amount),
          description: description,
          fromAccountId: accountId,
          toAccountId: toAccountId,
        })
      : actions.recordPurchase({
          date: date,
          amount: Number(amount),
          description: description,
          accountId: accountId,
          categoryId: categoryId,
          tags: tags,
          repeat: repeat && { startDate: repeat.startDate, intervalType: repeat.intervalType, frequency: Number(repeat.frequency) },
        });

  const save = async () => {
    setBusy(true);
    const result = await record();
    setBusy(false);
    if (!result.isSuccess) {
      notify(translate(tk.payUpcoming.failed));
      return;
    }
    notify(translate(transfer ? tk.newTransaction.transferSaved : tk.newTransaction.saved));
    refresh();
    onClose();
  };

  if (options.accounts.length === 0)
    return (
      <Box sx={{ px: 3 }}>
        <EmptyList
          testID="new-transaction-missing"
          title={translate(tk.emptyStates.purchaseMissingAccount.title)}
          hint={translate(tk.newTransaction.missingAccountHint)}
        />
      </Box>
    );

  if (categories.length === 0 && type === 'expense')
    return (
      <Box sx={{ px: 3 }}>
        <EmptyList
          testID="new-transaction-missing"
          title={translate(tk.emptyStates.purchaseMissingCategory.title)}
          hint={translate(tk.newTransaction.missingCategoryHint)}
        />
      </Box>
    );

  return (
    <>
      <Stack spacing={2.5} sx={{ px: 3, pb: 3, flexGrow: 1, overflowY: 'auto' }}>
        <ToggleButtonGroup exclusive fullWidth size="small" value={type} onChange={(_, next: TransactionType | null) => changeType(next)}>
          <ToggleButton value="expense" sx={typeButton}>
            {translate(tk.purchase.transactionTypeSection.expense)}
          </ToggleButton>
          <ToggleButton value="gain" sx={typeButton}>
            {translate(tk.purchase.transactionTypeSection.income)}
          </ToggleButton>
          <ToggleButton value="transfer" sx={typeButton}>
            {translate(tk.purchase.transactionTypeSection.transfer)}
          </ToggleButton>
        </ToggleButtonGroup>
        <Box sx={{ p: 2.5, borderRadius: 3, border: 2, borderColor: 'primary.main', bgcolor: 'background.default' }}>
          <Typography component="label" htmlFor="new-transaction-amount" sx={{ fontSize: 13, color: 'text.secondary' }}>
            {translate(tk.purchase.basicSection.amount)}
          </Typography>
          <InputBase
            id="new-transaction-amount"
            value={amount}
            onChange={(event) => setAmount(event.target.value)}
            onFocus={(event) => event.target.select()}
            fullWidth
            inputProps={{ inputMode: 'decimal', 'data-testid': 'new-transaction-amount' }}
            sx={{ fontSize: 40, fontWeight: 700, color: amountColor }}
          />
        </Box>
        <TextField
          type="date"
          label={translate(tk.purchase.basicSection.date)}
          value={date}
          onChange={(event) => setDate(event.target.value)}
          slotProps={{ inputLabel: { shrink: true }, htmlInput: { 'data-testid': 'new-transaction-date' } }}
        />
        {transfer ? (
          <>
            <AccountSelect
              label={translate(tk.newTransaction.from)}
              testID="new-transaction-from"
              accounts={options.accounts}
              value={accountId}
              onChange={setAccountId}
            />
            <AccountSelect
              label={translate(tk.newTransaction.to)}
              testID="new-transaction-to"
              accounts={options.accounts}
              value={toAccountId}
              onChange={setToAccountId}
            />
            {sameAccount && (
              <Alert severity="warning" data-testid="new-transaction-same-account">
                {translate(tk.newTransaction.sameAccount)}
              </Alert>
            )}
          </>
        ) : (
          <AccountSelect
            label={translate(tk.purchase.basicSection.account)}
            testID="new-transaction-account"
            accounts={options.accounts}
            value={accountId}
            onChange={setAccountId}
          />
        )}
        {transfer ? null : categories.length === 0 ? (
          <Alert severity="info" data-testid="new-transaction-missing">
            {translate(tk.emptyStates.purchaseMissingCategory.title)}
          </Alert>
        ) : (
          <TextField
            select
            label={translate(tk.purchase.basicSection.category)}
            value={categoryId}
            onChange={(event) => setCategoryId(event.target.value)}
            slotProps={{ select: { SelectDisplayProps: { 'data-testid': 'new-transaction-category' } as object } }}
          >
            {categories.map((category) => (
              <MenuItem key={category.id} value={category.id}>
                <Stack direction="row" alignItems="center" spacing={1.25}>
                  <Box aria-hidden sx={{ width: 10, height: 10, borderRadius: '50%', bgcolor: category.color, flexShrink: 0 }} />
                  <span>{category.name}</span>
                </Stack>
              </MenuItem>
            ))}
          </TextField>
        )}
        {!transfer && categoryId !== '' && <TagsField categoryId={categoryId} tags={tags} onChange={setTags} />}
        <TextField
          label={translate(tk.newTransaction.description)}
          placeholder={
            transfer
              ? translate(tk.newTransaction.transferDescription, { from: accountName(accountId), to: accountName(toAccountId) })
              : translate(tk.newTransaction.descriptionPlaceholder)
          }
          value={description}
          onChange={(event) => setDescription(event.target.value)}
          slotProps={{ inputLabel: { shrink: true }, htmlInput: { 'data-testid': 'new-transaction-description' } }}
        />
        {!transfer && <RepeatField draft={repeat} startDate={date} onChange={setRepeat} />}
      </Stack>
      <Divider />
      <Stack spacing={1.5} sx={{ p: 3 }}>
        <Button
          variant="contained"
          disableElevation
          fullWidth
          disabled={busy || !valid}
          data-testid="new-transaction-save"
          onClick={() => void save()}
          sx={footerButton}
        >
          {translate(tk.newTransaction.save)}
        </Button>
        <Button
          variant="outlined"
          color="inherit"
          fullWidth
          data-testid="new-transaction-cancel"
          onClick={onClose}
          sx={{ ...footerButton, borderColor: 'divider' }}
        >
          {translate(tk.common.cancel)}
        </Button>
      </Stack>
    </>
  );
};

const NewTransactionContent = ({ accountId, onClose, notify }: { accountId?: string; onClose: () => void; notify: Notify }) => {
  const options = useNewTransactionOptions();

  if (options.isLoading)
    return (
      <Box sx={{ px: 3 }}>
        <ListSkeleton testID="new-transaction-loading" rows={4} />
      </Box>
    );

  if (options.isFailure)
    return (
      <Box sx={{ px: 3 }}>
        <Alert severity="error" data-testid="new-transaction-error">
          {translate(tk.payUpcoming.failed)}
        </Alert>
      </Box>
    );

  return <NewTransactionForm options={options.value} accountId={accountId} onClose={onClose} notify={notify} />;
};

type NewTransactionFlyoutProps = { open: boolean; accountId?: string; onClose: () => void; notify: Notify };

export const NewTransactionFlyout = ({ open, accountId, onClose, notify }: NewTransactionFlyoutProps) => (
  <Drawer
    anchor="right"
    open={open}
    onClose={onClose}
    slotProps={{ paper: { 'data-testid': 'new-transaction', sx: { width: { xs: '100%', sm: 420 } } } as object }}
  >
    {open && (
      <Stack sx={{ height: '100%' }}>
        <Stack direction="row" alignItems="center" spacing={2} sx={{ px: 3, pt: 3, pb: 2 }}>
          <Typography component="h2" sx={{ flexGrow: 1, fontSize: 22, fontWeight: 700 }}>
            {translate(tk.newTransaction.title)}
          </Typography>
          <IconButton aria-label={translate(tk.newTransaction.close)} data-testid="new-transaction-close" onClick={onClose}>
            <CloseIcon />
          </IconButton>
        </Stack>
        <NewTransactionContent accountId={accountId} onClose={onClose} notify={notify} />
      </Stack>
    )}
  </Drawer>
);
