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
import { translate } from '@/shared/i18n/translate';
import { useNewTransactionActions } from '../api/use-new-transaction-actions';
import { useNewTransactionOptions } from '../api/use-new-transaction-options';
import type { NewTransactionOptions, TransactionCategoryOption } from '../domain/new-transaction-options';
import { isAmountInput } from './amount-input';
import type { Notify } from './DashboardToast';
import { EmptyList } from './EmptyList';
import { ListSkeleton } from './ListSkeleton';
import { isRepeatDraftValid, type RepeatDraft, RepeatField } from './RepeatField';
import { TagsField } from './TagsField';

const footerButton = { py: 1.25, borderRadius: 2.5, fontSize: 15, fontWeight: 600, textTransform: 'none' };

type CategoryType = TransactionCategoryOption['type'];

const today = () => new Date().toLocaleDateString('en-CA');

type NewTransactionFormProps = { options: NewTransactionOptions; onClose: () => void; notify: Notify };

const NewTransactionForm = ({ options, onClose, notify }: NewTransactionFormProps) => {
  const actions = useNewTransactionActions();
  const { refresh } = useApiRefresh();
  const categoriesOf = (type: CategoryType) => options.categories.filter((category) => category.type === type);

  const [type, setType] = useState<CategoryType>('expense');
  const [amount, setAmount] = useState('0.00');
  const [date, setDate] = useState(today);
  const [accountId, setAccountId] = useState(options.accounts[0]?.id ?? '');
  const [categoryId, setCategoryId] = useState(categoriesOf('expense')[0]?.id ?? '');
  const [tags, setTags] = useState<string[]>([]);
  const [description, setDescription] = useState('');
  const [repeat, setRepeat] = useState<RepeatDraft | null>(null);
  const [busy, setBusy] = useState(false);

  const categories = categoriesOf(type);
  const validAmount = isAmountInput(amount);
  const validRepeat = repeat === null || isRepeatDraftValid(repeat);
  const amountColor = type === 'gain' ? 'success.main' : 'error.main';

  const changeType = (next: CategoryType | null) => {
    if (next === null) return;
    setType(next);
    setCategoryId(categoriesOf(next)[0]?.id ?? '');
  };

  const save = async () => {
    setBusy(true);
    const result = await actions.recordPurchase({
      date: date,
      amount: Number(amount),
      description: description,
      accountId: accountId,
      categoryId: categoryId,
      tags: tags,
      repeat: repeat && { startDate: repeat.startDate, intervalType: repeat.intervalType, frequency: Number(repeat.frequency) },
    });
    setBusy(false);
    if (!result.isSuccess) {
      notify(translate(tk.payUpcoming.failed));
      return;
    }
    notify(translate(tk.newTransaction.saved));
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
        <ToggleButtonGroup exclusive fullWidth size="small" value={type} onChange={(_, next: CategoryType | null) => changeType(next)}>
          <ToggleButton value="expense" sx={{ textTransform: 'none', fontSize: 15 }}>
            {translate(tk.purchase.transactionTypeSection.expense)}
          </ToggleButton>
          <ToggleButton value="gain" sx={{ textTransform: 'none', fontSize: 15 }}>
            {translate(tk.purchase.transactionTypeSection.income)}
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
        <TextField
          select
          label={translate(tk.purchase.basicSection.account)}
          value={accountId}
          onChange={(event) => setAccountId(event.target.value)}
          slotProps={{ select: { SelectDisplayProps: { 'data-testid': 'new-transaction-account' } as object } }}
        >
          {options.accounts.map((account) => (
            <MenuItem key={account.id} value={account.id}>
              {account.name}
            </MenuItem>
          ))}
        </TextField>
        {categories.length === 0 ? (
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
        {categoryId !== '' && <TagsField categoryId={categoryId} tags={tags} onChange={setTags} />}
        <TextField
          label={translate(tk.newTransaction.description)}
          placeholder={translate(tk.newTransaction.descriptionPlaceholder)}
          value={description}
          onChange={(event) => setDescription(event.target.value)}
          slotProps={{ inputLabel: { shrink: true }, htmlInput: { 'data-testid': 'new-transaction-description' } }}
        />
        <RepeatField draft={repeat} startDate={date} onChange={setRepeat} />
      </Stack>
      <Divider />
      <Stack spacing={1.5} sx={{ p: 3 }}>
        <Button
          variant="contained"
          disableElevation
          fullWidth
          disabled={busy || !validAmount || !validRepeat || date === '' || categoryId === ''}
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

const NewTransactionContent = ({ onClose, notify }: { onClose: () => void; notify: Notify }) => {
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

  return <NewTransactionForm options={options.value} onClose={onClose} notify={notify} />;
};

type NewTransactionFlyoutProps = { open: boolean; onClose: () => void; notify: Notify };

export const NewTransactionFlyout = ({ open, onClose, notify }: NewTransactionFlyoutProps) => (
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
        <NewTransactionContent onClose={onClose} notify={notify} />
      </Stack>
    )}
  </Drawer>
);
