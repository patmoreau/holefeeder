import { AppButton } from '@/shared/presentation/components/app/AppButton';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { ExpoColumn } from '@/shared/presentation/components/expo/ExpoColumn';
import { ExpoFieldSection } from '@/shared/presentation/components/expo/ExpoFieldSection';
import { AppIcon } from '@/shared/presentation/core/app-icons';

export const TestComponentsButtonSection = () => {
  return (
    <ExpoFieldSection title={'AppButton'}>
      <AppField icon={AppIcon.add} label="Primary">
        <ExpoColumn spacing={8} alignment="end">
          <AppButton icon={AppIcon.add} variant="primary" onPress={() => alert('Test Button Clicked')} />
          <AppButton icon={AppIcon.add} label="Click me!" variant="primary" onPress={() => alert('Test Button Clicked')} />
          <AppButton label="Click me!" variant="primary" onPress={() => alert('Test Button Clicked')} />
        </ExpoColumn>
      </AppField>
      <AppField icon={AppIcon.add} label="Secondary">
        <AppButton label="Click me!" variant="secondary" onPress={() => alert('Test Button Clicked')} />
      </AppField>
      <AppField icon={AppIcon.add} label="Destructive">
        <AppButton label="Click me!" variant="destructive" onPress={() => alert('Test Button Clicked')} />
      </AppField>
      <AppField icon={AppIcon.add} label="Link">
        <AppButton icon={AppIcon.expand} iconPosition="right" label="Click me!" variant="link" onPress={() => alert('Test Button Clicked')} />
      </AppField>
      <AppField icon={AppIcon.add} label="Disabled">
        <AppButton icon={AppIcon.back} label="Click me!" disabled onPress={() => alert('You should not see this Button Clicked')} />
      </AppField>
    </ExpoFieldSection>
  );
};
