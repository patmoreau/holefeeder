import { useState } from 'react';
import { AppChip } from '@/shared/presentation/components/app/AppChip';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { ExpoFieldSection } from '@/shared/presentation/components/expo/ExpoFieldSection';
import { AppIcon } from '@/shared/presentation/core/app-icons';

export const TestComponentsChipSection = () => {
  const [chipSelection, setChipSelection] = useState(false);
  return (
    <ExpoFieldSection title={'AppChip'}>
      <AppField icon={AppIcon.back} label="AppChip">
        <AppChip label="Test Button" selected={chipSelection} onPress={() => setChipSelection(!chipSelection)} />
      </AppField>
    </ExpoFieldSection>
  );
};
