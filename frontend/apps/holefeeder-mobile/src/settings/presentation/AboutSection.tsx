import Constants from 'expo-constants';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppVersion } from '@/settings/core/app-version';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

export function AboutSection() {
  const { t } = useTranslation();

  return (
    <AppFieldSection title={t(tk.settings.aboutSection.title)}>
      <AppField icon={AppIconMap.version} label={t(tk.settings.aboutSection.version)}>
        <AppText variant={'footnote'} testID="settings-version">
          {AppVersion.label(Constants.expoConfig)}
        </AppText>
      </AppField>
    </AppFieldSection>
  );
}
