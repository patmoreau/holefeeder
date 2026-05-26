import { today } from '@holefeeder/shared/core';
import { useState } from 'react';
import { AppDatePicker } from '@/shared/presentation/components/app/AppDatePicker';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppPicker, PickerOption } from '@/shared/presentation/components/app/AppPicker';
import { ExpoFieldSection } from '@/shared/presentation/components/expo/ExpoFieldSection';
import { AppIcon } from '@/shared/presentation/core/app-icons';

type PickerType<T> = PickerOption & { value: T };
const pickerOptions: PickerType<string>[] = [
  { id: 'menu1', value: 'menu' },
  { id: 'menu2', value: 'medium menu' },
  { id: 'menu3', value: 'a very incredibly super super very long menu' },
];

export const TestComponentsPickerSection = () => {
  const [date, setDate] = useState(today());
  const [pickerValue, setPickerValue] = useState<PickerType<string>>(pickerOptions[0]);

  return (
    <ExpoFieldSection title={'AppDatePicker'}>
      <AppField icon={AppIcon.back} label="AppDatePicker">
        <AppDatePicker selectedDate={date} onDateSelected={(d) => setDate(d)} />
      </AppField>
      <AppField icon={AppIcon.category} label="Menu">
        <AppPicker
          variant={'menu'}
          options={pickerOptions}
          onOptionLabel={(option) => option.value}
          selectedOption={pickerValue}
          onSelectOption={setPickerValue}
        />
      </AppField>
      <AppField icon={AppIcon.category} label="Segmented" variant={'large'}>
        <AppPicker
          variant={'segmented'}
          options={pickerOptions}
          onOptionLabel={(option) => option.value}
          selectedOption={pickerValue}
          onSelectOption={setPickerValue}
        />
      </AppField>
    </ExpoFieldSection>
  );
};
