import React, { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppField } from '@/shared/presentation/AppField';
import { AppPicker, PickerOption } from '@/shared/presentation/components/AppPicker';
import { AppIcons } from '@/shared/presentation/icons';

type FrequencyOption = PickerOption & {
  value: number;
};

type Props = {
  selectedFrequency: number;
  onSelectFrequency: (frequency: number) => void;
  error?: string;
};

export function FrequencyField({ selectedFrequency, onSelectFrequency, error }: Props) {
  const { t } = useTranslation();

  const options = useMemo<FrequencyOption[]>(() => {
    const types = [1, 2, 3, 4, 5, 6, 7, 8, 9];
    return types.map((type) => ({
      id: type,
      value: type,
    }));
  }, []);

  const selectedOption = options.find((opt) => opt.value === selectedFrequency) ?? options[0];

  return (
    <AppField label={t(tk.purchase.cashflowSection.frequency)} icon={AppIcons.frequency} error={error}>
      <AppPicker
        options={options}
        selectedOption={selectedOption}
        onSelectOption={(option) => onSelectFrequency(option.value)}
        onOptionLabel={(option) => option.value.toString()}
      />
    </AppField>
  );
}
