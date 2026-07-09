import { useState } from 'react';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppSwitch } from '@/shared/presentation/components/native/AppSwitch';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

export const TestComponentsSwitchSection = () => {
  const [switchValue, setSwitchValue] = useState(false);
  return (
    <AppFieldSection title={'AppSwitch'}>
      <AppField icon={AppIconMap.back} label="Switch">
        <AppSwitch value={switchValue} onChange={setSwitchValue} />
      </AppField>
      <AppField icon={AppIconMap.back} label="Disabled Switch">
        <AppSwitch value={switchValue} onChange={setSwitchValue} disabled />
      </AppField>
    </AppFieldSection>
  );
};
