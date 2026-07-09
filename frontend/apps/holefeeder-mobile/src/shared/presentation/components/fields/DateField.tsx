import { DateOnly } from '@holefeeder/shared/core';
import { AppDatePicker } from '@/shared/presentation/components/native/AppDatePicker';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

type Props = {
  label: string;
  selectedDate?: DateOnly;
  onDateSelected: (date: DateOnly) => void;
  error?: string;
};

export function DateField({ label, selectedDate, onDateSelected, error }: Props) {
  return (
    <AppField label={label} icon={AppIconMap.calendar} error={error}>
      <AppDatePicker selectedDate={selectedDate} onDateSelected={onDateSelected} />
    </AppField>
  );
}
