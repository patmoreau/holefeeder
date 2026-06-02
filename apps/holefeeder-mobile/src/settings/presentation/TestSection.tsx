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
              // @ts-expect-error -- route not in typed manifest
              router.push({ pathname: '/+not-found' });
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
