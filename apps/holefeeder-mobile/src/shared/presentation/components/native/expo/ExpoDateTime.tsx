import { DatePicker, DatePickerProps } from '@expo/ui/swift-ui';
import { withDate } from '@holefeeder/shared/core';
import { ExpoModifierConfig } from '@/shared/presentation/components/native/expo/expo-modifier-config';

export type ExpoDateTimePickerProps = {
  date?: Date;
  onDateChange: (date: Date) => void;
  modifiers?: ExpoModifierConfig[];
};

export const ExpoDateTimePicker = ({ date, onDateChange, modifiers }: ExpoDateTimePickerProps) => {
  const datePickerProps: DatePickerProps = {
    onDateChange: onDateChange,
    selection: date ? withDate(date).toDate() : undefined,
    modifiers,
  };
  return <DatePicker {...datePickerProps} />;
};
