import { useState } from 'react';
import { AppIconMap } from '@/shared/presentation/components/app/app-icon-map';
import { AppChip } from '@/shared/presentation/components/app/AppChip';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppFieldSection } from '@/shared/presentation/components/app/AppFieldSection';

export const TestComponentsChipSection = () => {
  const [chipSelection, setChipSelection] = useState(false);
  return (
    <AppFieldSection title={'AppChip'}>
      <AppField icon={AppIconMap.back} label="AppChip">
        <AppChip label="Test Button" selected={chipSelection} onPress={() => setChipSelection(!chipSelection)} />
      </AppField>
    </AppFieldSection>
  );
};
