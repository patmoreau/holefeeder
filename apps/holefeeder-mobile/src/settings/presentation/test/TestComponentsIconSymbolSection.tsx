import { useState } from 'react';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { ExpoFieldSection } from '@/shared/presentation/components/expo/ExpoFieldSection';
import { ExpoPicker } from '@/shared/presentation/components/expo/ExpoPicker';
import { AppIcon, AppIconsMappings } from '@/shared/presentation/core/app-icons';

const iconEntries = Object.entries(AppIcon) as [string, AppIconsMappings][];

export const TestComponentsIconSymbolSection = () => {
  const [selectedKey, setSelectedKey] = useState(iconEntries[0][0]);
  const selectedIcon = AppIcon[selectedKey];

  return (
    <ExpoFieldSection title={'IconSymbol'}>
      <AppField icon={selectedIcon}>
        <ExpoPicker selectedValue={selectedKey} onValueChange={setSelectedKey}>
          {iconEntries.map(([key]) => (
            <ExpoPicker.Item key={key} label={key} value={key} />
          ))}
        </ExpoPicker>
      </AppField>
    </ExpoFieldSection>
  );
};
