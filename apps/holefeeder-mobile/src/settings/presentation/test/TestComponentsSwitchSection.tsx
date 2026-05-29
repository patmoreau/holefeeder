import { useState } from 'react';
import { AppIconMap } from '@/shared/presentation/components/app/app-icon-map';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppFieldSection } from '@/shared/presentation/components/app/AppFieldSection';
import { AppSwitch } from '@/shared/presentation/components/app/AppSwitch';

export const TestComponentsSwitchSection = () => {
  const [switchValue, setSwitchValue] = useState(false);
  return (
    <AppFieldSection title={'AppSwitch'}>
      <AppField icon={AppIconMap.back} label="Switch">
        <AppSwitch value={switchValue} onChange={setSwitchValue} />
      </AppField>
      <AppField icon={AppIconMap.back} label="Disabled Switch">
        <AppSwitch value={switchValue} onChange={setSwitchValue} readonly />
      </AppField>
    </AppFieldSection>
  );
};
