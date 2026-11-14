import { DateIntervalType, DateOnly, tk } from '@holefeeder/shared/core';
import type { Settings } from '@holefeeder/shared/core';
import CloseIcon from '@mui/icons-material/Close';
import { Alert, Button, Divider, Drawer, IconButton, MenuItem, Stack, TextField, Typography } from '@mui/material';
import { useState } from 'react';
import { useApiRefresh } from '@/shared/api/api-refresh';
import { translate } from '@/shared/i18n/translate';
import type { Notify } from '@/shared/presentation/DashboardToast';
import { useSaveBudgetSettings } from '../api/use-save-budget-settings';

const testID = 'budget-settings';

const footerButton = { py: 1.25, borderRadius: 2.5, fontSize: 15, fontWeight: 600, textTransform: 'none' };

const repeatingIntervals: DateIntervalType[] = ['daily', 'weekly', 'monthly', 'yearly'];

const isWholeFrequency = (frequency: string) => /^[1-9]\d*$/.test(frequency);

type BudgetSettingsFormProps = { initial: Settings; onClose: () => void; notify: Notify };

const BudgetSettingsForm = ({ initial, onClose, notify }: BudgetSettingsFormProps) => {
  const saveBudgetSettings = useSaveBudgetSettings();
  const { refresh } = useApiRefresh();
  const [effectiveDate, setEffectiveDate] = useState<string>(initial.effectiveDate);
  const [intervalType, setIntervalType] = useState(initial.intervalType);
  const [frequency, setFrequency] = useState(String(initial.frequency));
  const [failed, setFailed] = useState(false);
  const [busy, setBusy] = useState(false);

  const intervals = repeatingIntervals.includes(initial.intervalType) ? repeatingIntervals : [...repeatingIntervals, initial.intervalType];
  const valid = effectiveDate !== '' && isWholeFrequency(frequency);

  const save = async () => {
    setBusy(true);
    setFailed(false);
    const result = await saveBudgetSettings({
      effectiveDate: DateOnly.valid(effectiveDate),
      intervalType: intervalType,
      frequency: Number(frequency),
    });
    setBusy(false);
    if (!result.isSuccess) {
      setFailed(true);
      return;
    }
    notify(translate(tk.budgetSettings.saved));
    refresh();
    onClose();
  };

  return (
    <>
      <Stack spacing={2.5} sx={{ px: 3, pt: 1, pb: 3, flexGrow: 1, overflowY: 'auto' }}>
        <TextField
          type="date"
          label={translate(tk.budgetSettings.date)}
          value={effectiveDate}
          onChange={(event) => setEffectiveDate(event.target.value)}
          slotProps={{ inputLabel: { shrink: true }, htmlInput: { 'data-testid': `${testID}-effective-date` } }}
        />
        <TextField
          select
          label={translate(tk.purchase.cashflowSection.intervalType)}
          value={intervalType}
          onChange={(event) => setIntervalType(event.target.value as DateIntervalType)}
          slotProps={{ select: { SelectDisplayProps: { 'data-testid': `${testID}-interval` } as object } }}
        >
          {intervals.map((interval) => (
            <MenuItem key={interval} value={interval}>
              {translate(DateIntervalType.labelKey[interval])}
            </MenuItem>
          ))}
        </TextField>
        <TextField
          label={translate(tk.purchase.cashflowSection.frequency)}
          value={frequency}
          onChange={(event) => setFrequency(event.target.value)}
          slotProps={{ htmlInput: { inputMode: 'numeric', 'data-testid': `${testID}-frequency` } }}
        />
        <Typography sx={{ fontSize: 13, color: 'text.secondary' }}>{translate(tk.budgetSettings.hint)}</Typography>
        {failed && (
          <Alert severity="error" data-testid={`${testID}-error`}>
            {translate(tk.budgetSettings.failed)}
          </Alert>
        )}
      </Stack>
      <Divider />
      <Stack spacing={1.5} sx={{ p: 3 }}>
        <Button
          variant="contained"
          disableElevation
          fullWidth
          disabled={busy || !valid}
          data-testid={`${testID}-save`}
          onClick={() => void save()}
          sx={footerButton}
        >
          {translate(tk.common.save)}
        </Button>
        <Button
          variant="outlined"
          color="inherit"
          fullWidth
          data-testid={`${testID}-cancel`}
          onClick={onClose}
          sx={{ ...footerButton, borderColor: 'divider' }}
        >
          {translate(tk.common.cancel)}
        </Button>
      </Stack>
    </>
  );
};

type BudgetSettingsFlyoutProps = { open: boolean; settings: Settings; onClose: () => void; notify: Notify };

export const BudgetSettingsFlyout = ({ open, settings, onClose, notify }: BudgetSettingsFlyoutProps) => (
  <Drawer
    anchor="right"
    open={open}
    onClose={onClose}
    slotProps={{ paper: { 'data-testid': testID, sx: { width: { xs: '100%', sm: 420 } } } as object }}
  >
    {open && (
      <Stack sx={{ height: '100%' }}>
        <Stack direction="row" alignItems="center" spacing={2} sx={{ px: 3, pt: 3, pb: 2 }}>
          <Typography component="h2" sx={{ flexGrow: 1, fontSize: 22, fontWeight: 700 }}>
            {translate(tk.budgetSettings.section)}
          </Typography>
          <IconButton aria-label={translate(tk.accountEdit.close)} data-testid={`${testID}-close`} onClick={onClose}>
            <CloseIcon />
          </IconButton>
        </Stack>
        <BudgetSettingsForm initial={settings} onClose={onClose} notify={notify} />
      </Stack>
    )}
  </Drawer>
);
