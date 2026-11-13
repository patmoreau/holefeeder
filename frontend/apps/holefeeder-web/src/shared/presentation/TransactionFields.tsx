import { type CategoryType, tk } from '@holefeeder/shared/core';
import { Box, Button, Divider, InputBase, MenuItem, Stack, TextField, ToggleButton, ToggleButtonGroup, Typography } from '@mui/material';
import type { TransactionAccountOption, TransactionCategoryOption } from '@/shared/domain/new-transaction-options';
import { translate } from '@/shared/i18n/translate';

const footerButton = { py: 1.25, borderRadius: 2.5, fontSize: 15, fontWeight: 600, textTransform: 'none' };

const typeButton = { textTransform: 'none', fontSize: 15 };

export type TransactionType = CategoryType | 'transfer';

type TransactionTypeToggleProps = { value: TransactionType; withTransfer: boolean; onChange: (type: TransactionType) => void };

export const TransactionTypeToggle = ({ value, withTransfer, onChange }: TransactionTypeToggleProps) => (
  <ToggleButtonGroup
    exclusive
    fullWidth
    size="small"
    value={value}
    onChange={(_, next: TransactionType | null) => {
      if (next !== null) onChange(next);
    }}
  >
    <ToggleButton value="expense" sx={typeButton}>
      {translate(tk.purchase.transactionTypeSection.expense)}
    </ToggleButton>
    <ToggleButton value="gain" sx={typeButton}>
      {translate(tk.purchase.transactionTypeSection.income)}
    </ToggleButton>
    {withTransfer && (
      <ToggleButton value="transfer" sx={typeButton}>
        {translate(tk.purchase.transactionTypeSection.transfer)}
      </ToggleButton>
    )}
  </ToggleButtonGroup>
);

type AmountFieldProps = { testID: string; value: string; color: string; onChange: (value: string) => void };

export const AmountField = ({ testID, value, color, onChange }: AmountFieldProps) => (
  <Box sx={{ p: 2.5, borderRadius: 3, border: 2, borderColor: 'primary.main', bgcolor: 'background.default' }}>
    <Typography component="label" htmlFor={testID} sx={{ fontSize: 13, color: 'text.secondary' }}>
      {translate(tk.purchase.basicSection.amount)}
    </Typography>
    <InputBase
      id={testID}
      value={value}
      onChange={(event) => onChange(event.target.value)}
      onFocus={(event) => event.target.select()}
      fullWidth
      inputProps={{ inputMode: 'decimal', 'data-testid': testID }}
      sx={{ fontSize: 40, fontWeight: 700, color: color }}
    />
  </Box>
);

type AccountSelectProps = {
  label: string;
  testID: string;
  accounts: TransactionAccountOption[];
  value: string;
  onChange: (id: string) => void;
};

export const AccountSelect = ({ label, testID, accounts, value, onChange }: AccountSelectProps) => (
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

type CategorySelectProps = { testID: string; categories: TransactionCategoryOption[]; value: string; onChange: (id: string) => void };

export const CategorySelect = ({ testID, categories, value, onChange }: CategorySelectProps) => (
  <TextField
    select
    label={translate(tk.purchase.basicSection.category)}
    value={value}
    onChange={(event) => onChange(event.target.value)}
    slotProps={{ select: { SelectDisplayProps: { 'data-testid': testID } as object } }}
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
);

type FlyoutFooterProps = { testID: string; saveLabel: string; disabled: boolean; onSave: () => void; onCancel: () => void };

export const FlyoutFooter = ({ testID, saveLabel, disabled, onSave, onCancel }: FlyoutFooterProps) => (
  <>
    <Divider />
    <Stack spacing={1.5} sx={{ p: 3 }}>
      <Button
        variant="contained"
        disableElevation
        fullWidth
        disabled={disabled}
        data-testid={`${testID}-save`}
        onClick={onSave}
        sx={footerButton}
      >
        {saveLabel}
      </Button>
      <Button
        variant="outlined"
        color="inherit"
        fullWidth
        data-testid={`${testID}-cancel`}
        onClick={onCancel}
        sx={{ ...footerButton, borderColor: 'divider' }}
      >
        {translate(tk.common.cancel)}
      </Button>
    </Stack>
  </>
);
