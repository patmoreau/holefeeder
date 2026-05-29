import { DateOnly } from '@holefeeder/shared/core';
import { AppDatePicker } from '@/shared/presentation/components/app/AppDatePicker';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppIconMap } from '../components/app/app-icon-map';

type Props = {
  label: string;
  selectedDate: DateOnly | null;
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
