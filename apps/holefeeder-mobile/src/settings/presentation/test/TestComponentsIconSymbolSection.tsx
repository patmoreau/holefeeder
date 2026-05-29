import { useState } from 'react';
import { AppIconMap, UniversalIcon } from '@/shared/presentation/components/app/app-icon-map';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppFieldSection } from '@/shared/presentation/components/app/AppFieldSection';
import { AppPicker, PickerOption } from '@/shared/presentation/components/app/AppPicker';

type IconOption = PickerOption & { icon: UniversalIcon };

const iconOptions: IconOption[] = (Object.entries(AppIconMap).filter(([key]) => key !== 'select') as [string, UniversalIcon][]).map(
  ([key, icon]) => ({ id: key, icon })
);

export const TestComponentsIconSymbolSection = () => {
  const [selectedOption, setSelectedOption] = useState<IconOption>(iconOptions[0]);

  return (
    <AppFieldSection title={'IconSymbol'}>
      <AppField icon={selectedOption.icon} label={'Select an icon'}>
        <AppPicker
          options={iconOptions}
          onOptionLabel={(option) => option.id}
          selectedOption={selectedOption}
          onSelectOption={setSelectedOption}
        />
      </AppField>
    </AppFieldSection>
  );
};
