import { AppIconMap } from '@/shared/presentation/components/app/app-icon-map';
import { AppButton } from '@/shared/presentation/components/app/AppButton';
import { AppColumn } from '@/shared/presentation/components/app/AppColumn';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppFieldSection } from '@/shared/presentation/components/app/AppFieldSection';

export const TestComponentsButtonSection = () => {
  return (
    <AppFieldSection title={'AppButton'}>
      <AppField icon={AppIconMap.add} label="Primary">
        <AppColumn spacing={8} alignment="end">
          <AppButton icon={AppIconMap.add} variant="primary" onPress={() => alert('Test Button Clicked')} />
          <AppButton icon={AppIconMap.add} label="Click me!" variant="primary" onPress={() => alert('Test Button Clicked')} />
          <AppButton label="Click me!" variant="primary" onPress={() => alert('Test Button Clicked')} />
        </AppColumn>
      </AppField>
      <AppField icon={AppIconMap.add} label="Secondary">
        <AppButton label="Click me!" variant="secondary" onPress={() => alert('Test Button Clicked')} />
      </AppField>
      <AppField icon={AppIconMap.add} label="Destructive">
        <AppButton label="Click me!" variant="destructive" onPress={() => alert('Test Button Clicked')} />
      </AppField>
      <AppField icon={AppIconMap.add} label="Link">
        <AppButton
          icon={AppIconMap.expand}
          iconPosition="right"
          label="Click me!"
          variant="link"
          onPress={() => alert('Test Button Clicked')}
        />
      </AppField>
      <AppField icon={AppIconMap.add} label="Disabled">
        <AppButton icon={AppIconMap.back} label="Click me!" disabled onPress={() => alert('You should not see this Button Clicked')} />
      </AppField>
    </AppFieldSection>
  );
};
