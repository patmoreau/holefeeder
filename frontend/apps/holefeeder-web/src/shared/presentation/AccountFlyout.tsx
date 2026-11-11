import { type Result, tk } from '@holefeeder/shared/core';
import CloseIcon from '@mui/icons-material/Close';
import {
  Alert,
  Box,
  Button,
  Divider,
  Drawer,
  FormControlLabel,
  IconButton,
  InputBase,
  MenuItem,
  Stack,
  Switch,
  TextField,
  Typography,
} from '@mui/material';
import { useState } from 'react';
import { type AccountType, AccountTypes, accountTypeKey } from '@/shared/domain/account-type';
import { translate } from '@/shared/i18n/translate';
import { isAmountInput } from './amount-input';

const footerButton = { py: 1.25, borderRadius: 2.5, fontSize: 15, fontWeight: 600, textTransform: 'none' };

export type AccountDraft = {
  type: AccountType;
  name: string;
  openDate: string;
  openBalance: number;
  description: string;
  favorite: boolean;
};

type AccountFormProps = {
  testID: string;
  submitLabel: string;
  initial: AccountDraft;
  onSubmit: (draft: AccountDraft) => Promise<Result<unknown>>;
  onSaved: () => void;
  onClose: () => void;
};

const AccountForm = ({ testID, submitLabel, initial, onSubmit, onSaved, onClose }: AccountFormProps) => {
  const [openBalance, setOpenBalance] = useState(initial.openBalance.toFixed(2));
  const [name, setName] = useState(initial.name);
  const [type, setType] = useState<AccountType>(initial.type);
  const [openDate, setOpenDate] = useState(initial.openDate);
  const [description, setDescription] = useState(initial.description);
  const [favorite, setFavorite] = useState(initial.favorite);
  const [nameTouched, setNameTouched] = useState(false);
  const [failed, setFailed] = useState(false);
  const [busy, setBusy] = useState(false);

  const nameMissing = name.trim() === '';
  const valid = isAmountInput(openBalance) && openDate !== '';

  const save = async () => {
    setNameTouched(true);
    if (nameMissing) return;
    setBusy(true);
    setFailed(false);
    const result = await onSubmit({
      type: type,
      name: name.trim(),
      openDate: openDate,
      openBalance: Number(openBalance),
      description: description,
      favorite: favorite,
    });
    setBusy(false);
    if (!result.isSuccess) {
      setFailed(true);
      return;
    }
    onSaved();
  };

  return (
    <>
      <Stack spacing={2.5} sx={{ px: 3, pb: 3, flexGrow: 1, overflowY: 'auto' }}>
        <Box sx={{ p: 2.5, borderRadius: 3, border: 2, borderColor: 'primary.main', bgcolor: 'background.default', textAlign: 'center' }}>
          <Typography component="label" htmlFor={`${testID}-balance`} sx={{ fontSize: 13, color: 'text.secondary' }}>
            {translate(tk.accountEdit.openBalance)}
          </Typography>
          <InputBase
            id={`${testID}-balance`}
            value={openBalance}
            onChange={(event) => setOpenBalance(event.target.value)}
            onFocus={(event) => event.target.select()}
            fullWidth
            inputProps={{ inputMode: 'decimal', 'data-testid': `${testID}-balance`, style: { textAlign: 'center' } }}
            sx={{ fontSize: 40, fontWeight: 700 }}
          />
        </Box>
        <TextField
          label={translate(tk.accountEdit.name)}
          value={name}
          onChange={(event) => setName(event.target.value)}
          onBlur={() => setNameTouched(true)}
          error={nameTouched && nameMissing}
          helperText={nameTouched && nameMissing ? translate(tk.accountEdit.errors.nameRequired) : undefined}
          slotProps={{ htmlInput: { 'data-testid': `${testID}-name`, maxLength: 100 } }}
        />
        <TextField
          select
          label={translate(tk.accountEdit.type)}
          value={type}
          onChange={(event) => setType(event.target.value as AccountType)}
          slotProps={{ select: { SelectDisplayProps: { 'data-testid': `${testID}-type` } as object } }}
        >
          {Object.values(AccountTypes).map((accountType) => (
            <MenuItem key={accountType} value={accountType}>
              {translate(accountTypeKey[accountType])}
            </MenuItem>
          ))}
        </TextField>
        <TextField
          type="date"
          label={translate(tk.accountEdit.openDate)}
          value={openDate}
          onChange={(event) => setOpenDate(event.target.value)}
          slotProps={{ inputLabel: { shrink: true }, htmlInput: { 'data-testid': `${testID}-open-date` } }}
        />
        <TextField
          label={translate(tk.accountEdit.descriptionLabel)}
          placeholder={translate(tk.accountEdit.optional)}
          value={description}
          onChange={(event) => setDescription(event.target.value)}
          slotProps={{ inputLabel: { shrink: true }, htmlInput: { 'data-testid': `${testID}-description` } }}
        />
        <Box sx={{ px: 2, py: 1, borderRadius: 3, bgcolor: 'background.default' }}>
          <FormControlLabel
            labelPlacement="start"
            sx={{ m: 0, width: '100%', justifyContent: 'space-between', gap: 2 }}
            control={
              <Switch
                checked={favorite}
                onChange={(event) => setFavorite(event.target.checked)}
                slotProps={{ input: { 'data-testid': `${testID}-favorite` } as object }}
              />
            }
            label={
              <Stack>
                <Typography sx={{ fontSize: 15 }}>{translate(tk.accountEdit.favorite)}</Typography>
                <Typography sx={{ fontSize: 13, color: 'text.secondary' }}>{translate(tk.accountEdit.favoriteHint)}</Typography>
              </Stack>
            }
          />
        </Box>
        {failed && (
          <Alert severity="error" data-testid={`${testID}-error`}>
            {translate(tk.accountEdit.failed)}
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
          {submitLabel}
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

type AccountFlyoutProps = AccountFormProps & { open: boolean; title: string };

export const AccountFlyout = ({ open, title, ...form }: AccountFlyoutProps) => (
  <Drawer
    anchor="right"
    open={open}
    onClose={form.onClose}
    slotProps={{ paper: { 'data-testid': form.testID, sx: { width: { xs: '100%', sm: 420 } } } as object }}
  >
    {open && (
      <Stack sx={{ height: '100%' }}>
        <Stack direction="row" alignItems="center" spacing={2} sx={{ px: 3, pt: 3, pb: 2 }}>
          <Typography component="h2" sx={{ flexGrow: 1, fontSize: 22, fontWeight: 700 }}>
            {title}
          </Typography>
          <IconButton aria-label={translate(tk.accountEdit.close)} data-testid={`${form.testID}-close`} onClick={form.onClose}>
            <CloseIcon />
          </IconButton>
        </Stack>
        <AccountForm {...form} />
      </Stack>
    )}
  </Drawer>
);
