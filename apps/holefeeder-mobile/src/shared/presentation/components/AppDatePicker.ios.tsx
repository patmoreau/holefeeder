import { DatePicker, DatePickerProps } from '@expo/ui/swift-ui';
import { StyleProp, ViewStyle } from 'react-native';
import { DateOnly } from '@/shared/core/date-only';
import { today, withDate } from '@/shared/core/with-date';
import { AppHost } from '@/shared/presentation/components/AppHost.ios';

export type AppDatePickerProps = {
  selectedDate: DateOnly | null;
  onDateSelected: (date: DateOnly) => void;
  style?: StyleProp<ViewStyle>;
};

export const AppDatePicker = ({ selectedDate, onDateSelected }: AppDatePickerProps) => {
  const initialDate = withDate(selectedDate || today()).toDate();

  const datePickerProps: DatePickerProps = {
    onDateChange: (date: Date) => onDateSelected(withDate(date).toDateOnly()),
    displayedComponents: ['date' as const],
    selection: initialDate,
  };

  return (
    <AppHost>
      <DatePicker {...datePickerProps} />
    </AppHost>
  );
};
