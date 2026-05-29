import { LanguageType } from '@holefeeder/shared/core';
import React, { useEffect, useMemo, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { useLanguage } from '@/shared/language/core/use-language';
import { AppIconMap } from '@/shared/presentation/components/app/app-icon-map';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppPicker, PickerOption } from '@/shared/presentation/components/app/AppPicker';

const tkTypes: Record<LanguageType, string> = {
  [LanguageType.en]: tk.displaySection.english,
  [LanguageType.fr]: tk.displaySection.french,
};

type LanguageOption = PickerOption & {
  value: LanguageType;
};

export const LanguageField = () => {
  const { t } = useTranslation();
  const { language, setLanguage } = useLanguage();

  const options = useMemo<LanguageOption[]>(() => {
    const types = Object.values(LanguageType) as LanguageType[];
    return types.map((type) => ({
      id: type,
      value: type,
    }));
  }, []);

  const [selectedOption, setSelectedOption] = useState<LanguageOption>(options[0]);

  useEffect(() => {
    setSelectedOption(options.find((opt) => opt.value === language) ?? options[0]);
  }, [language, options]);

  return (
    <AppField label={t(tk.displaySection.language)} icon={AppIconMap.language}>
      <AppPicker
        options={options}
        selectedOption={selectedOption}
        onSelectOption={(option) => setLanguage(option.value)}
        onOptionLabel={(option) => t(tkTypes[option.value])}
      />
    </AppField>
  );
};
