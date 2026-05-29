import { AppIconMap } from '@/shared/presentation/components/app/app-icon-map';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppFieldSection } from '@/shared/presentation/components/app/AppFieldSection';
import { AppLoadingIndicator } from '@/shared/presentation/components/app/AppLoadingIndicator';

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
