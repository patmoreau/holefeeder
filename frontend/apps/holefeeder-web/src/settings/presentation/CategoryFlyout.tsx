import { type CategoryType, tk } from '@holefeeder/shared/core';
import CloseIcon from '@mui/icons-material/Close';
import {
  Alert,
  Box,
  ButtonBase,
  Drawer,
  FormControlLabel,
  IconButton,
  InputAdornment,
  Stack,
  Switch,
  TextField,
  Typography,
} from '@mui/material';
import { useState } from 'react';
import { useApiRefresh } from '@/shared/api/api-refresh';
import { translate } from '@/shared/i18n/translate';
import { isAmountInput } from '@/shared/presentation/amount-input';
import type { Notify } from '@/shared/presentation/DashboardToast';
import { FlyoutFooter, TransactionTypeToggle } from '@/shared/presentation/TransactionFields';
import { useSaveCategory } from '../api/use-save-category';
import type { Category } from '../domain/category';

const testID = 'category';

const nameTaken = 'category-name-taken';

const swatches = ['#34C759', '#30B0C7', '#007AFF', '#5856D6', '#AF52DE', '#FF2D55', '#FF3B30', '#FF9500', '#FFCC00', '#A2845E'];

const ColorSwatches = ({ value, onChange }: { value: string; onChange: (color: string) => void }) => {
  const colors = swatches.some((color) => color.toUpperCase() === value.toUpperCase()) ? swatches : [...swatches, value];

  return (
    <Stack spacing={1.25}>
      <Typography id={`${testID}-color-label`} sx={{ fontSize: 13, color: 'text.secondary' }}>
        {translate(tk.categoryEdit.color)}
      </Typography>
      <Box role="radiogroup" aria-labelledby={`${testID}-color-label`} sx={{ display: 'flex', flexWrap: 'wrap', gap: 1.25 }}>
        {colors.map((color) => {
          const checked = color.toUpperCase() === value.toUpperCase();
          return (
            <ButtonBase
              key={color}
              role="radio"
              aria-checked={checked}
              aria-label={color}
              onClick={() => onChange(color)}
              sx={{
                width: 28,
                height: 28,
                borderRadius: '50%',
                bgcolor: color,
                boxShadow: (theme) =>
                  checked ? `0 0 0 3px ${theme.palette.background.paper}, 0 0 0 5px ${theme.palette.text.primary}` : 'none',
              }}
            />
          );
        })}
      </Box>
    </Stack>
  );
};

type CategoryFormProps = { category: Category | null; onClose: () => void; notify: Notify };

const CategoryForm = ({ category, onClose, notify }: CategoryFormProps) => {
  const saveCategory = useSaveCategory();
  const { refresh } = useApiRefresh();
  const [type, setType] = useState<CategoryType>(category?.type ?? 'expense');
  const [name, setName] = useState(category?.name ?? '');
  const [budget, setBudget] = useState((category?.budgetAmount ?? 0).toFixed(2));
  const [color, setColor] = useState(category?.color ?? swatches[0]);
  const [favorite, setFavorite] = useState(category?.favorite ?? false);
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  const valid = name.trim() !== '' && isAmountInput(budget);

  const save = async () => {
    setBusy(true);
    setError(null);
    const result = await saveCategory(category?.id ?? null, {
      name: name.trim(),
      type: type,
      color: color,
      budgetAmount: Number(budget),
      favorite: favorite,
    });
    setBusy(false);
    if (!result.isSuccess) {
      setError(translate(result.errors.includes(nameTaken) ? tk.categories.nameTaken : tk.categories.failed));
      return;
    }
    notify(translate(tk.categories.saved));
    refresh();
    onClose();
  };

  return (
    <>
      <Stack spacing={2.5} sx={{ px: 3, pt: 1, pb: 3, flexGrow: 1, overflowY: 'auto' }}>
        <TransactionTypeToggle value={type} withTransfer={false} onChange={(next) => setType(next as CategoryType)} />
        <TextField
          label={translate(tk.categoryEdit.name)}
          value={name}
          onChange={(event) => setName(event.target.value)}
          slotProps={{ inputLabel: { shrink: true }, htmlInput: { 'data-testid': `${testID}-name` } }}
        />
        <TextField
          label={translate(tk.categoryEdit.budget)}
          value={budget}
          onChange={(event) => setBudget(event.target.value)}
          onFocus={(event) => event.target.select()}
          helperText={translate(tk.categories.budgetHint)}
          slotProps={{
            inputLabel: { shrink: true },
            htmlInput: { inputMode: 'decimal', 'data-testid': `${testID}-budget` },
            input: { endAdornment: <InputAdornment position="end">$</InputAdornment> },
          }}
        />
        <ColorSwatches value={color} onChange={setColor} />
        <FormControlLabel
          labelPlacement="start"
          control={
            <Switch
              checked={favorite}
              onChange={(event) => setFavorite(event.target.checked)}
              slotProps={{ input: { 'data-testid': `${testID}-favorite` } as object }}
            />
          }
          label={
            <Box>
              <Typography sx={{ fontSize: 15 }}>{translate(tk.categories.favorite)}</Typography>
              <Typography sx={{ fontSize: 13, color: 'text.secondary' }}>{translate(tk.categories.favoriteHint)}</Typography>
            </Box>
          }
          sx={{ mx: 0, justifyContent: 'space-between', '& .MuiFormControlLabel-label': { flexGrow: 1 } }}
        />
        {error !== null && (
          <Alert severity="error" data-testid={`${testID}-error`}>
            {error}
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

export type CategoryEditing = { category: Category | null } | null;

type CategoryFlyoutProps = { editing: CategoryEditing; onClose: () => void; notify: Notify };

export const CategoryFlyout = ({ editing, onClose, notify }: CategoryFlyoutProps) => (
  <Drawer
    anchor="right"
    open={editing !== null}
    onClose={onClose}
    slotProps={{ paper: { 'data-testid': testID, sx: { width: { xs: '100%', sm: 420 } } } as object }}
  >
    {editing !== null && (
      <Stack sx={{ height: '100%' }}>
        <Stack direction="row" alignItems="center" spacing={2} sx={{ px: 3, pt: 3, pb: 2 }}>
          <Typography component="h2" sx={{ flexGrow: 1, fontSize: 22, fontWeight: 700 }}>
            {translate(editing.category === null ? tk.categories.newTitle : tk.categories.editTitle)}
          </Typography>
          <IconButton aria-label={translate(tk.newTransaction.close)} data-testid={`${testID}-close`} onClick={onClose}>
            <CloseIcon />
          </IconButton>
        </Stack>
        <CategoryForm key={editing.category?.id ?? 'new'} category={editing.category} onClose={onClose} notify={notify} />
      </Stack>
    )}
  </Drawer>
);
