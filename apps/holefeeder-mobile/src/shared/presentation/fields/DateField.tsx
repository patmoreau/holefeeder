import { DateOnly } from '@/shared/core/date-only';
import { AppField } from '@/shared/presentation/AppField';
import { AppDatePicker } from '@/shared/presentation/components/AppDatePicker';
import { AppIcons } from '@/shared/presentation/icons';

type Props = {
  label: string;
  selectedDate: DateOnly | null;
  onDateSelected: (date: DateOnly) => void;
  error?: string;
};

export function DateField({ label, selectedDate, onDateSelected, error }: Props) {
  return (
    <AppField label={label} icon={AppIcons.calendar} error={error}>
      <AppDatePicker selectedDate={selectedDate} onDateSelected={onDateSelected} />
    </AppField>
  );
}
