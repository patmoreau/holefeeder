import { useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { Category } from '@/flows/core/categories/category';
import { tk } from '@/i18n/translations';
import { CategoryType } from '@/shared/core/category-type';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppMenu } from '@/shared/presentation/components/native/AppMenu';
import { AppModifiers } from '@/shared/presentation/components/native/AppModifiers';
import { AppRow } from '@/shared/presentation/components/native/AppRow';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap, UniversalIcon } from '@/shared/presentation/core/app-icon-map';

type Props = {
  categories: Category[];
  selectedCategory: Category;
  onSelectCategory: (category: Category) => void;
  variant?: CategoryType;
  error?: string;
};

export function CategoryField({ categories, selectedCategory, onSelectCategory, variant, error }: Props) {
  const { t } = useTranslation();
  const filteredCategories = variant ? categories.filter((category) => category.type === variant) : categories;
  const isSelectedCategoryInFiltered = filteredCategories.some((category) => category.id === selectedCategory.id);

  useEffect(() => {
    if (!isSelectedCategoryInFiltered && filteredCategories.length > 0) {
      onSelectCategory(filteredCategories[0]);
    }
  }, [isSelectedCategoryInFiltered, filteredCategories, onSelectCategory]);

  const selected = isSelectedCategoryInFiltered ? selectedCategory : filteredCategories[0];

  const menuLabel = selected ? (
    <AppRow spacing={6} alignment="center">
      <AppIcon name={selected.favorite ? AppIconMap.favorite : AppIconMap.circle} size={14} color={selected.color} />
      <AppText modifiers={[AppModifiers.foregroundStyle('accentColor')]}>{selected.name}</AppText>
      <AppIcon name={AppIconMap.dropdown} size={14} />
    </AppRow>
  ) : (
    ''
  );

  const items = filteredCategories.map((category) => {
    let icon: UniversalIcon = AppIconMap.circle;
    if (category.favorite) icon = AppIconMap.favorite;
    if (selected === category) icon = AppIconMap.selected;

    return (
      <AppButton key={category.id} label={category.name} onPress={() => onSelectCategory(category)} icon={icon} iconColor={category.color} />
    );
  });

  return (
    <AppField label={t(tk.purchase.basicSection.category)} icon={AppIconMap.category} error={error}>
      <AppMenu label={menuLabel}>{items}</AppMenu>
    </AppField>
  );
}
