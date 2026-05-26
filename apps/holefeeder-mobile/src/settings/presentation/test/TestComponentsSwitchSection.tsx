import { useState } from 'react';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppSwitch } from '@/shared/presentation/components/app/AppSwitch';
import { ExpoFieldSection } from '@/shared/presentation/components/expo/ExpoFieldSection';
import { AppIcon } from '@/shared/presentation/core/app-icons';

export const TestComponentsSwitchSection = () => {
  const [switchValue, setSwitchValue] = useState(false);
  return (
    <ExpoFieldSection title={'AppSwitch'}>
      <AppField icon={AppIcon.back} label="Switch">
        <AppSwitch value={switchValue} onChange={setSwitchValue} />
      </AppField>
      <AppField icon={AppIcon.back} label="Disabled Switch">
        <AppSwitch value={switchValue} onChange={setSwitchValue} readonly />
      </AppField>
    </ExpoFieldSection>
  );
};
