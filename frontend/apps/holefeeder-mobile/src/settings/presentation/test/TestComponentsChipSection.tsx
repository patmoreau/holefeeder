import { useState } from 'react';
import { AppChip } from '@/shared/presentation/components/native/AppChip';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

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
