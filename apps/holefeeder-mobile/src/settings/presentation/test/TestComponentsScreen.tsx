import { Stack, useNavigation } from 'expo-router';
import { useState } from 'react';
import { AppErrorSheet } from '@/shared/presentation/components/app/AppErrorSheet';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppSwitch } from '@/shared/presentation/components/app/AppSwitch';
import { ExpoFieldGroup } from '@/shared/presentation/components/expo/ExpoFieldGroup';
import { ExpoFieldSection } from '@/shared/presentation/components/expo/ExpoFieldSection';
import { ExpoHost } from '@/shared/presentation/components/expo/ExpoHost';
import { AppIcon } from '@/shared/presentation/core/app-icons';
import { AppIcons } from '@/shared/presentation/icons';
import { TestComponentsButtonSection } from './TestComponentsButtonSection';
import { TestComponentsChipSection } from './TestComponentsChipSection';
import { TestComponentsIconSymbolSection } from './TestComponentsIconSymbolSection';
import { TestComponentsLoadingIndicatorSection } from './TestComponentsLoadingIndicatorSection';
import { TestComponentsPickerSection } from './TestComponentsPickerSection';
import { TestComponentsSwitchSection } from './TestComponentsSwitchSection';
import { TestComponentsTextSection } from './TestComponentsTextSection';

const TestComponentsScreen = () => {
  const [showError, setShowError] = useState(false);
  const navigation = useNavigation();
  return (
    <>
      <Stack.Toolbar placement="left">
        <Stack.Toolbar.Button icon={AppIcons.back} onPress={() => navigation.goBack()} />
      </Stack.Toolbar>

      <ExpoHost style={{ flex: 1 }}>
        <ExpoFieldGroup>
          <TestComponentsButtonSection />
          <TestComponentsChipSection />
          <TestComponentsPickerSection />
          <TestComponentsSwitchSection />
          <TestComponentsTextSection />
          <TestComponentsIconSymbolSection />
          <TestComponentsLoadingIndicatorSection />
          <ExpoFieldSection title={'ErrorSheet'}>
            <AppField icon={AppIcon.warning} label={'Show error sheet'}>
              <AppSwitch value={showError} onChange={setShowError} />
            </AppField>
          </ExpoFieldSection>
        </ExpoFieldGroup>
        <AppErrorSheet showError={showError} setShowError={setShowError} error={'noInternetConnection'} />
      </ExpoHost>
    </>
  );
};

export default TestComponentsScreen;
