import { tk } from '@holefeeder/shared/core';
import { FormControlLabel, MenuItem, Stack, Switch, TextField, Typography } from '@mui/material';
import type { RepeatIntervalType } from '@/shared/api/use-new-transaction-actions';
import { translate } from '@/shared/i18n/translate';

export type RepeatDraft = { startDate: string; intervalType: RepeatIntervalType; frequency: string };

const intervalTypes: RepeatIntervalType[] = ['daily', 'weekly', 'monthly', 'yearly'];

export const isRepeatDraftValid = (draft: RepeatDraft) => draft.startDate !== '' && /^[1-9]\d*$/.test(draft.frequency);

type RepeatFieldProps = {
  draft: RepeatDraft | null;
  startDate: string;
  onChange: (draft: RepeatDraft | null) => void;
};

export const RepeatField = ({ draft, startDate, onChange }: RepeatFieldProps) => (
  <Stack spacing={2}>
    <FormControlLabel
      data-testid="new-transaction-repeat"
      control={
        <Switch
          checked={draft !== null}
          onChange={(event) => onChange(event.target.checked ? { startDate: startDate, intervalType: 'monthly', frequency: '1' } : null)}
        />
      }
      label={
        <Stack>
          <Typography sx={{ fontSize: 15 }}>{translate(tk.newTransaction.repeat)}</Typography>
          <Typography sx={{ fontSize: 13, color: 'text.secondary' }}>{translate(tk.newTransaction.repeatHint)}</Typography>
        </Stack>
      }
      labelPlacement="start"
      sx={{ mx: 0, justifyContent: 'space-between', gap: 2 }}
    />
    {draft && (
      <>
        <TextField
          type="date"
          label={translate(tk.newTransaction.starts)}
          value={draft.startDate}
          onChange={(event) => onChange({ ...draft, startDate: event.target.value })}
          slotProps={{ inputLabel: { shrink: true }, htmlInput: { 'data-testid': 'new-transaction-repeat-starts' } }}
        />
        <Stack direction="row" spacing={1.5}>
          <TextField
            label={translate(tk.newTransaction.every)}
            value={draft.frequency}
            onChange={(event) => onChange({ ...draft, frequency: event.target.value })}
            slotProps={{ htmlInput: { inputMode: 'numeric', 'data-testid': 'new-transaction-repeat-frequency' } }}
            sx={{ width: 96, flexShrink: 0 }}
          />
          <TextField
            select
            fullWidth
            value={draft.intervalType}
            onChange={(event) => onChange({ ...draft, intervalType: event.target.value as RepeatIntervalType })}
            slotProps={{ select: { SelectDisplayProps: { 'data-testid': 'new-transaction-repeat-interval' } as object } }}
          >
            {intervalTypes.map((intervalType) => (
              <MenuItem key={intervalType} value={intervalType}>
                {translate(tk.newTransaction.units[intervalType])}
              </MenuItem>
            ))}
          </TextField>
        </Stack>
      </>
    )}
  </Stack>
);
