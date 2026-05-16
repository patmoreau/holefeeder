import React, { useEffect, useMemo, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { Logger } from '@/shared/core/logger/logger';
import { AppField } from '@/shared/presentation/AppField';
import { AppPicker, PickerOption } from '@/shared/presentation/components/AppPicker';
import { AppIcons } from '@/shared/presentation/icons';
import { useTheme } from '@/shared/theme/core/use-theme';
import { ThemeMode } from '@/types/theme/theme';

const logger = Logger.create('ThemeField');

const tkTypes: Record<ThemeMode, string> = {
  [ThemeMode.dark]: tk.displaySection.dark,
  [ThemeMode.light]: tk.displaySection.light,
  [ThemeMode.system]: tk.displaySection.system,
};

type ThemeOption = PickerOption & {
  value: ThemeMode;
};

export const ThemeField = () => {
  const { t } = useTranslation();
  const { setThemeMode, themeMode } = useTheme();
  const handleThemeChange = async (theme: ThemeOption) => {
    await setThemeMode(theme.value);
  };

  const options = useMemo<ThemeOption[]>(() => {
    const types = Object.values(ThemeMode) as ThemeMode[];
    return types.map((type) => ({
      id: type,
      value: type,
    }));
  }, []);

  const [selectedOption, setSelectedOption] = useState<ThemeOption>(options[0]);

  useEffect(() => {
    setSelectedOption(options.find((opt) => opt.value === themeMode) ?? options[0]);
  }, [themeMode, options]);

  return (
    <AppField label={t(tk.displaySection.theme)} icon={AppIcons.theme}>
      <AppPicker
        options={options}
        selectedOption={selectedOption}
        onSelectOption={(option) => handleThemeChange(option).catch(logger.error)}
        onOptionLabel={(option) => t(tkTypes[option.value])}
      />
    </AppField>
  );
};
