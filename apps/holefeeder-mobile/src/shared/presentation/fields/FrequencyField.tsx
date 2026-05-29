import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppPicker, PickerOption } from '@/shared/presentation/components/app/AppPicker';
import { AppIconMap } from '../components/app/app-icon-map';

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
      id: type.toString(),
      value: type,
    }));
  }, []);

  const selectedOption = options.find((opt) => opt.value === selectedFrequency) ?? options[0];

  return (
    <AppField label={t(tk.purchase.cashflowSection.frequency)} icon={AppIconMap.frequency} error={error}>
      <AppPicker
        options={options}
        selectedOption={selectedOption}
        onSelectOption={(option) => onSelectFrequency(option.value)}
        onOptionLabel={(option) => option.value.toString()}
      />
    </AppField>
  );
}
