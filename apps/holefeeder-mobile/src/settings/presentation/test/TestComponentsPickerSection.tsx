import { today } from '@holefeeder/shared/core';
import { useState } from 'react';
import { AppDatePicker } from '@/shared/presentation/components/native/AppDatePicker';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppPicker, PickerOption } from '@/shared/presentation/components/native/AppPicker';
import { AppSegmentedMenu } from '@/shared/presentation/components/native/AppSegmentedMenu';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

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
    <AppFieldSection title={'AppPicker'}>
      <AppField icon={AppIconMap.back} label="Date">
        <AppDatePicker selectedDate={date} onDateSelected={(d) => setDate(d)} />
      </AppField>
      <AppField icon={AppIconMap.category} label="Menu">
        <AppPicker
          options={pickerOptions}
          onOptionLabel={(option) => option.value}
          selectedOption={pickerValue}
          onSelectOption={setPickerValue}
        />
      </AppField>
      <AppSegmentedMenu
        options={pickerOptions}
        onOptionLabel={(option) => option.value}
        selectedOption={pickerValue}
        onSelectOption={setPickerValue}
      />
    </AppFieldSection>
  );
};
