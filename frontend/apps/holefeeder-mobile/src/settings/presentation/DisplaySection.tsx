import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { LanguageField } from '@/settings/presentation/fields/LanguageField';
import { ThemeField } from '@/settings/presentation/fields/ThemeField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';

export function DisplaySection() {
  const { t } = useTranslation();

  return (
    <AppFieldSection title={t(tk.displaySection.title)}>
      <LanguageField />
      <ThemeField />
    </AppFieldSection>
  );
}
