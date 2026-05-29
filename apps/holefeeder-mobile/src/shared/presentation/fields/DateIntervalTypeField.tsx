import { DateIntervalType, DateIntervalTypes } from '@holefeeder/shared/core';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppPicker, PickerOption } from '@/shared/presentation/components/app/AppPicker';
import { AppIconMap } from '../components/app/app-icon-map';

const tkTypes: Record<DateIntervalType, string> = {
  [DateIntervalTypes.daily]: tk.dateIntervalTypePicker.daily,
  [DateIntervalTypes.weekly]: tk.dateIntervalTypePicker.weekly,
  [DateIntervalTypes.monthly]: tk.dateIntervalTypePicker.monthly,
  [DateIntervalTypes.yearly]: tk.dateIntervalTypePicker.yearly,
  [DateIntervalTypes.oneTime]: tk.dateIntervalTypePicker.oneTime,
};

type DateIntervalTypeOption = PickerOption & {
  value: DateIntervalType;
};

type Props = {
  selectedDateIntervalType: DateIntervalType | null;
  onSelectDateIntervalType: (dateIntervalType: DateIntervalType) => void;
  error?: string;
};

export function DateIntervalTypeField({ selectedDateIntervalType, onSelectDateIntervalType, error }: Props) {
  const { t } = useTranslation();

  const options = useMemo<DateIntervalTypeOption[]>(() => {
    const types = Object.values(DateIntervalTypes) as DateIntervalType[];
    return types.map((type) => ({
      id: type,
      value: type,
    }));
  }, []);
  const selectedOption = options.find((opt) => opt.value === selectedDateIntervalType) ?? options[0];

  return (
    <AppField label={t(tk.purchase.cashflowSection.intervalType)} icon={AppIconMap.calendar} error={error}>
      <AppPicker
        options={options}
        selectedOption={selectedOption}
        onSelectOption={(option) => onSelectDateIntervalType(option.value)}
        onOptionLabel={(option) => t(tkTypes[option.value])}
      />
    </AppField>
  );
}
