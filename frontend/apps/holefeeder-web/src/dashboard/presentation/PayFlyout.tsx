import { tk } from '@holefeeder/shared/core';
import CloseIcon from '@mui/icons-material/Close';
import { Box, Button, Divider, Drawer, FormControlLabel, IconButton, InputBase, Stack, Switch, TextField, Typography } from '@mui/material';
import { useState } from 'react';
import { translate } from '@/shared/i18n/translate';
import { isAmountInput } from '@/shared/presentation/amount-input';
import { currency, mediumDate } from '@/shared/presentation/tone';
import type { PayChanges } from '../api/use-upcoming-actions';
import type { DashboardUpcomingFlow } from '../domain/dashboard-upcoming-flow';

const footerButton = { py: 1.25, borderRadius: 2.5, fontSize: 15, fontWeight: 600, textTransform: 'none' };

type PayFormProps = {
  flow: DashboardUpcomingFlow;
  busy: boolean;
  onClose: () => void;
  onPay: (changes: PayChanges) => void;
};

const PayForm = ({ flow, busy, onClose, onPay }: PayFormProps) => {
  const [amount, setAmount] = useState(flow.amount.toFixed(2));
  const [date, setDate] = useState<string>(flow.date);
  const [updateRecurring, setUpdateRecurring] = useState(false);

  const validAmount = isAmountInput(amount);
  const paid = validAmount ? Number(amount) : flow.amount;
  const changed = validAmount && paid !== flow.amount;
  const amountColor = flow.categoryType === 'gain' ? 'success.main' : 'error.main';

  return (
    <Stack sx={{ height: '100%' }}>
      <Stack direction="row" alignItems="flex-start" spacing={2} sx={{ px: 3, pt: 3, pb: 2 }}>
        <Stack spacing={0.5} sx={{ flexGrow: 1, minWidth: 0 }}>
          <Typography component="h2" noWrap sx={{ fontSize: 22, fontWeight: 700 }}>
            {translate(tk.payUpcoming.titleFor, { title: flow.title })}
          </Typography>
          <Typography data-testid="pay-flyout-subtitle" sx={{ fontSize: 15, color: 'text.secondary' }}>
            {[flow.category, translate(tk.payUpcoming.due, { date: mediumDate(flow.date) })].filter(Boolean).join(' · ')}
          </Typography>
        </Stack>
        <IconButton aria-label={translate(tk.payUpcoming.close)} data-testid="pay-flyout-close" onClick={onClose} sx={{ mt: -0.5 }}>
          <CloseIcon />
        </IconButton>
      </Stack>
      <Stack spacing={2.5} sx={{ px: 3, pb: 3, flexGrow: 1, overflowY: 'auto' }}>
        <Box sx={{ p: 2.5, borderRadius: 3, border: 2, borderColor: 'primary.main', bgcolor: 'background.default' }}>
          <Typography component="label" htmlFor="pay-flyout-amount" sx={{ fontSize: 13, color: 'text.secondary' }}>
            {translate(tk.purchase.basicSection.amount)}
          </Typography>
          <InputBase
            id="pay-flyout-amount"
            value={amount}
            onChange={(event) => setAmount(event.target.value)}
            fullWidth
            inputProps={{ inputMode: 'decimal', 'data-testid': 'pay-flyout-amount' }}
            sx={{ fontSize: 40, fontWeight: 700, color: amountColor }}
          />
          {changed && (
            <Typography data-testid="pay-flyout-scheduled" sx={{ fontSize: 13, color: 'text.secondary' }}>
              {translate(tk.payUpcoming.scheduled, { amount: currency(flow.amount) })}
            </Typography>
          )}
        </Box>
        <TextField
          type="date"
          label={translate(tk.purchase.basicSection.date)}
          value={date}
          onChange={(event) => setDate(event.target.value)}
          slotProps={{ inputLabel: { shrink: true }, htmlInput: { 'data-testid': 'pay-flyout-date' } }}
        />
        {changed && (
          <FormControlLabel
            data-testid="pay-flyout-update-recurring"
            control={<Switch checked={updateRecurring} onChange={(event) => setUpdateRecurring(event.target.checked)} />}
            label={translate(tk.payUpcoming.updateRecurring, { amount: currency(paid) })}
            labelPlacement="start"
            sx={{ mx: 0, justifyContent: 'space-between', gap: 2 }}
          />
        )}
      </Stack>
      <Divider />
      <Stack spacing={1.5} sx={{ p: 3 }}>
        <Button
          variant="contained"
          disableElevation
          fullWidth
          disabled={busy || !validAmount || date === ''}
          data-testid="pay-flyout-pay"
          onClick={() => onPay({ date: date, amount: paid, updateRecurringAmount: changed && updateRecurring })}
          sx={footerButton}
        >
          {translate(tk.payUpcoming.pay)}
        </Button>
        <Button
          variant="outlined"
          color="inherit"
          fullWidth
          data-testid="pay-flyout-cancel"
          onClick={onClose}
          sx={{ ...footerButton, borderColor: 'divider' }}
        >
          {translate(tk.common.cancel)}
        </Button>
      </Stack>
    </Stack>
  );
};

type PayFlyoutProps = {
  flow: DashboardUpcomingFlow | null;
  busy: boolean;
  onClose: () => void;
  onPay: (flow: DashboardUpcomingFlow, changes: PayChanges) => void;
};

export const PayFlyout = ({ flow, busy, onClose, onPay }: PayFlyoutProps) => (
  <Drawer
    anchor="right"
    open={flow !== null}
    onClose={onClose}
    slotProps={{ paper: { 'data-testid': 'pay-flyout', sx: { width: { xs: '100%', sm: 420 } } } as object }}
  >
    {flow && <PayForm key={flow.cashflowId + flow.date} flow={flow} busy={busy} onClose={onClose} onPay={(changes) => onPay(flow, changes)} />}
  </Drawer>
);
