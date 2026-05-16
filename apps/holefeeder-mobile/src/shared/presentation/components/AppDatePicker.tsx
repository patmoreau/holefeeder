import React, { useEffect, useState } from 'react';
import { Modal, Pressable, StyleProp, Text, ViewStyle } from 'react-native';
import DateTimePicker, { DateType, useDefaultStyles } from 'react-native-ui-datepicker';
import { DateOnly } from '@/shared/core/date-only';
import { withDate } from '@/shared/core/with-date';
import { useStyles } from '@/shared/theme/core/use-styles';
import { borderRadius, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

export type AppDatePickerProps = {
  selectedDate: DateOnly | null;
  onDateSelected: (date: DateOnly) => void;
  style?: StyleProp<ViewStyle>;
};

const createStyles = (theme: Theme) => ({
  dateInput: {
    ...theme.typography.body,
    color: theme.colors.text,
    backgroundColor: 'transparent' as const,
    borderWidth: 0,
    outline: 'none' as const,
    padding: spacing.sm,
    minWidth: 120,
    textAlign: 'right' as const,
  },
  modalOverlay: {
    flex: 1,
    justifyContent: 'center' as const,
    alignItems: 'center' as const,
    backgroundColor: 'rgba(0, 0, 0, 0.5)',
  },
  modalContent: {
    backgroundColor: theme.colors.background,
    borderRadius: borderRadius.lg,
    padding: spacing.xl,
    width: '90%' as const,
    maxWidth: 400,
    shadowColor: '#000' as const,
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.25,
    shadowRadius: 4,
    elevation: 5,
  },
});

export function AppDatePicker({ selectedDate, onDateSelected, style }: AppDatePickerProps) {
  const defaultStyles = useDefaultStyles();
  const styles = useStyles(createStyles);
  const [selected, setSelected] = useState<DateType>(selectedDate);
  const [showPicker, setShowPicker] = useState(false);

  useEffect(() => {
    setSelected(selectedDate);
  }, [selectedDate]);

  const handleDateChange = (params: { date: DateType }) => {
    const date = params.date;
    setSelected(date);
    if (date) {
      let dateOnly: DateOnly | undefined;
      if (typeof date === 'string') {
        const dateOnlyResult = DateOnly.create(date);
        if (dateOnlyResult.isSuccess) {
          dateOnly = dateOnlyResult.value;
        }
      } else if (typeof date === 'number') {
        dateOnly = withDate(new Date(date)).toDateOnly();
      } else if (date instanceof Date) {
        dateOnly = withDate(date).toDateOnly();
      }

      if (dateOnly) {
        onDateSelected(dateOnly);
        setShowPicker(false);
      }
    }
  };

  const displayDate = selectedDate
    ? new Date(Number(selectedDate.slice(0, 4)), Number(selectedDate.slice(5, 7)) - 1, Number(selectedDate.slice(8, 10))).toLocaleDateString(
        'en-US',
        { year: 'numeric', month: 'short', day: 'numeric' }
      )
    : 'Select Date';

  return (
    <>
      <Pressable onPress={() => setShowPicker(true)} style={style}>
        <Text style={styles.dateInput}>{displayDate}</Text>
      </Pressable>
      <Modal animationType="fade" transparent={true} visible={showPicker} onRequestClose={() => setShowPicker(false)}>
        <Pressable style={styles.modalOverlay} onPress={() => setShowPicker(false)}>
          <Pressable style={styles.modalContent} onPress={(e) => e.stopPropagation()}>
            <DateTimePicker mode="single" date={selected} onChange={handleDateChange} styles={defaultStyles} />
          </Pressable>
        </Pressable>
      </Modal>
    </>
  );
}
