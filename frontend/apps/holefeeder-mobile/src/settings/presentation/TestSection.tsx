import { router } from 'expo-router';
import { Dispatch, SetStateAction } from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

export const TestSection = ({ show, setShow }: { show: boolean; setShow: Dispatch<SetStateAction<boolean>> }) => {
  const { t } = useTranslation();

  return (
    <>
      <AppFieldSection title={t(tk.testSection.title)}>
        <AppField label={t(tk.testSection.notFoundPage)} icon={AppIconMap.warning}>
          <AppButton
            label={t(tk.testSection.goTo)}
            variant={'link'}
            onPress={() => {
              // params, because the generated route manifest declares them required
              // here. Without it the call has to typecheck too: CI has no generated
              // types, and a @ts-expect-error would then be an error itself.
              router.push({ pathname: '/+not-found', params: {} });
            }}
          />
        </AppField>
        <AppField label={t(tk.testSection.testComponents)} icon={AppIconMap.warning}>
          <AppButton label={t(tk.testSection.component)} variant={'link'} onPress={() => setShow(!show)} />
        </AppField>
      </AppFieldSection>
    </>
  );
};
