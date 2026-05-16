import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { LanguageField } from '@/settings/presentation/fields/LanguageField';
import { ThemeField } from '@/settings/presentation/fields/ThemeField';
import { AppSection } from '@/shared/presentation/AppSection';

export function DisplaySection() {
  const { t } = useTranslation();

  return (
    <AppSection title={t(tk.displaySection.title)}>
      <LanguageField />
      <ThemeField />
    </AppSection>
  );
}
