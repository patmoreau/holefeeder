import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppLoadingIndicator } from '@/shared/presentation/components/app/AppLoadingIndicator';
import { ExpoFieldSection } from '@/shared/presentation/components/expo/ExpoFieldSection';
import { AppIcon } from '@/shared/presentation/core/app-icons';

export const TestComponentsLoadingIndicatorSection = () => {
  return (
    <ExpoFieldSection title={'LoadingIndicator'}>
      <AppField icon={AppIcon.tag} label={'Large'}>
        <AppLoadingIndicator variant={'primary'} size={'large'} />
      </AppField>
      <AppField icon={AppIcon.tag} label={'Small'}>
        <AppLoadingIndicator variant={'secondary'} size={'small'} />
      </AppField>
    </ExpoFieldSection>
  );
};
