import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

export const TestComponentsLoadingIndicatorSection = () => {
  return (
    <AppFieldSection title={'LoadingIndicator'}>
      <AppField icon={AppIconMap.tag} label={'Large'}>
        <AppLoadingIndicator variant={'primary'} size={'large'} />
      </AppField>
      <AppField icon={AppIconMap.tag} label={'Small'}>
        <AppLoadingIndicator variant={'secondary'} size={'small'} />
      </AppField>
    </AppFieldSection>
  );
};
