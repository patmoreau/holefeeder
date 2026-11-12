import { tk } from '@holefeeder/shared/core';
import CloseIcon from '@mui/icons-material/Close';
import { Alert, Box, Drawer, IconButton, InputAdornment, Stack, TextField, Typography } from '@mui/material';
import { useState } from 'react';
import { useApiRefresh } from '@/shared/api/api-refresh';
import { translate } from '@/shared/i18n/translate';
import { isAmountInput } from '@/shared/presentation/amount-input';
import type { Notify } from '@/shared/presentation/DashboardToast';
import { TagsField } from '@/shared/presentation/TagsField';
import { FlyoutFooter } from '@/shared/presentation/TransactionFields';
import { useModifyCashflow } from '../api/use-modify-cashflow';
import type { Cashflow } from '../domain/cashflow';
import { scheduleOf } from './cashflow-schedule';

const testID = 'cashflow';

type CashflowFormProps = { cashflow: Cashflow; onClose: () => void; notify: Notify };

const CashflowForm = ({ cashflow, onClose, notify }: CashflowFormProps) => {
  const modifyCashflow = useModifyCashflow();
  const { refresh } = useApiRefresh();
  const [amount, setAmount] = useState(cashflow.amount.toFixed(2));
  const [date, setDate] = useState<string>(cashflow.effectiveDate);
  const [description, setDescription] = useState(cashflow.description);
  const [tags, setTags] = useState(cashflow.tags);
  const [failed, setFailed] = useState(false);
  const [busy, setBusy] = useState(false);

  const valid = isAmountInput(amount) && date !== '';

  const save = async () => {
    setBusy(true);
    setFailed(false);
    const result = await modifyCashflow(cashflow.id, {
      amount: Number(amount),
      effectiveDate: date,
      description: description,
      tags: tags,
    });
    setBusy(false);
    if (!result.isSuccess) {
      setFailed(true);
      return;
    }
    notify(translate(tk.cashflows.saved));
    refresh();
    onClose();
  };

  return (
    <>
      <Stack spacing={2.5} sx={{ px: 3, pt: 1, pb: 3, flexGrow: 1, overflowY: 'auto' }}>
        <Box data-testid={`${testID}-summary`} sx={{ px: 2, py: 1.5, borderRadius: 2.5, bgcolor: 'background.default' }}>
          <Typography sx={{ fontSize: 15 }}>{`${cashflow.categoryName} · ${cashflow.accountName}`}</Typography>
          <Typography sx={{ fontSize: 13, color: 'text.secondary' }}>{scheduleOf(cashflow)}</Typography>
        </Box>
        <TextField
          label={translate(tk.purchase.basicSection.amount)}
          value={amount}
          onChange={(event) => setAmount(event.target.value)}
          onFocus={(event) => event.target.select()}
          slotProps={{
            inputLabel: { shrink: true },
            htmlInput: { inputMode: 'decimal', 'data-testid': `${testID}-amount` },
            input: { endAdornment: <InputAdornment position="end">$</InputAdornment> },
          }}
        />
        <TextField
          type="date"
          label={translate(tk.cashflows.startsOn)}
          value={date}
          onChange={(event) => setDate(event.target.value)}
          slotProps={{ inputLabel: { shrink: true }, htmlInput: { 'data-testid': `${testID}-date` } }}
        />
        <TextField
          label={translate(tk.newTransaction.description)}
          placeholder={translate(tk.newTransaction.descriptionPlaceholder)}
          value={description}
          onChange={(event) => setDescription(event.target.value)}
          slotProps={{ inputLabel: { shrink: true }, htmlInput: { 'data-testid': `${testID}-description` } }}
        />
        <TagsField testID={testID} categoryId={cashflow.categoryId} tags={tags} onChange={setTags} />
        {failed && (
          <Alert severity="error" data-testid={`${testID}-error`}>
            {translate(tk.cashflows.failed)}
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

type CashflowFlyoutProps = { cashflow: Cashflow | null; onClose: () => void; notify: Notify };

export const CashflowFlyout = ({ cashflow, onClose, notify }: CashflowFlyoutProps) => (
  <Drawer
    anchor="right"
    open={cashflow !== null}
    onClose={onClose}
    slotProps={{ paper: { 'data-testid': testID, sx: { width: { xs: '100%', sm: 420 } } } as object }}
  >
    {cashflow !== null && (
      <Stack sx={{ height: '100%' }}>
        <Stack direction="row" alignItems="center" spacing={2} sx={{ px: 3, pt: 3, pb: 2 }}>
          <Typography component="h2" sx={{ flexGrow: 1, fontSize: 22, fontWeight: 700 }}>
            {translate(tk.cashflows.editTitle)}
          </Typography>
          <IconButton aria-label={translate(tk.newTransaction.close)} data-testid={`${testID}-close`} onClick={onClose}>
            <CloseIcon />
          </IconButton>
        </Stack>
        <CashflowForm key={cashflow.id} cashflow={cashflow} onClose={onClose} notify={notify} />
      </Stack>
    )}
  </Drawer>
);
