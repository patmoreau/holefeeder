import { today } from '@holefeeder/shared/core';
import { useState } from 'react';
import { AppIconMap } from '@/shared/presentation/components/app/app-icon-map';
import { AppDatePicker } from '@/shared/presentation/components/app/AppDatePicker';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppFieldSection } from '@/shared/presentation/components/app/AppFieldSection';
import { AppPicker, PickerOption } from '@/shared/presentation/components/app/AppPicker';
import { AppSegmentedMenu } from '@/shared/presentation/components/app/AppSegmentedMenu';

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
      <AppField icon={AppIconMap.category} label="Segmented" variant={'large'}>
        <AppSegmentedMenu
          options={pickerOptions}
          onOptionLabel={(option) => option.value}
          selectedOption={pickerValue}
          onSelectOption={setPickerValue}
        />
      </AppField>
    </AppFieldSection>
  );
};
