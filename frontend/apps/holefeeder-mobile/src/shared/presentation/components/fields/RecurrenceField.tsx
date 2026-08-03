import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppPicker, PickerOption } from '@/shared/presentation/components/native/AppPicker';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

type RecurrenceOption = PickerOption & {
  value: number;
};

type Props = {
  selectedRecurrence: number;
  onSelectRecurrence: (recurrence: number) => void;
  error?: string;
};

export function RecurrenceField({ selectedRecurrence, onSelectRecurrence, error }: Props) {
  const { t } = useTranslation();

  const options = useMemo<RecurrenceOption[]>(() => {
    return Array.from({ length: 25 }, (_, value) => ({ id: value.toString(), value }));
  }, []);

  const selectedOption = options.find((opt) => opt.value === selectedRecurrence) ?? options[0];

  const label = (value: number) => (value === 0 ? t(tk.purchase.cashflowSection.recurrenceNever) : value.toString());

  return (
    <AppField label={t(tk.purchase.cashflowSection.recurrence)} icon={AppIconMap.expiresAt} error={error}>
      <AppPicker
        options={options}
        selectedOption={selectedOption}
        onSelectOption={(option) => onSelectRecurrence(option.value)}
        onOptionLabel={(option) => label(option.value)}
      />
    </AppField>
  );
}
