import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppRow } from '@/shared/presentation/components/native/AppRow';
import { AppTextInput } from '@/shared/presentation/components/native/AppTextInput';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useTheme } from '@/shared/theme/core/use-theme';

type FilterFieldProps = {
  filter: string;
  setFilter: (filter: string) => void;
  onSubmit: () => void;
};

export const FilterField = ({ filter, setFilter, onSubmit }: FilterFieldProps) => {
  const { t } = useTranslation();
  const { theme } = useTheme();

  return (
    <AppRow alignment={'center'}>
      <AppTextInput
        value={filter}
        onChangeText={setFilter}
        placeholder={t(tk.tagList.placeHolder)}
        onSubmitEditing={onSubmit}
        returnKeyType="search"
      />
      <AppButton icon={AppIconMap.add} color={theme.colors.primary} onPress={onSubmit} />
    </AppRow>
  );
};
