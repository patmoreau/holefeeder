import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppColorPicker } from '@/shared/presentation/components/native/AppColorPicker';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

type Props = {
  color: string;
  onColorChange: (color: string) => void;
};

export function CategoryColorField({ color, onColorChange }: Props) {
  const { t } = useTranslation();

  return (
    <AppField label={t(tk.categoryEdit.color)} icon={AppIconMap.category}>
      <AppColorPicker value={color} onChange={onColorChange} />
    </AppField>
  );
}
