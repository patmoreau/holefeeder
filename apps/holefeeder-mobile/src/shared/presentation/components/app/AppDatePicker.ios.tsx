import { DateOnly, today, withDate } from '@holefeeder/shared/core';
import { ExpoDateTimePicker, ExpoDateTimePickerProps } from '../expo/ExpoDateTime.ios';

export type AppDatePickerProps = ExpoDateTimePickerProps & {
  selectedDate: DateOnly | null;
  onDateSelected: (date: DateOnly) => void;
};

export const AppDatePicker = ({ selectedDate, onDateSelected }: AppDatePickerProps) => {
  const initialDate = withDate(selectedDate || today()).toDate();

  const datePickerProps: ExpoDateTimePickerProps = {
    onDateChange: (date: Date) => onDateSelected(withDate(date).toDateOnly()),
    displayedComponents: ['date' as const],
    selection: initialDate,
  };

  return <ExpoDateTimePicker {...datePickerProps} />;
};
