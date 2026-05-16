import React, { useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { Category } from '@/flows/core/categories/category';
import { CategoryType } from '@/flows/core/categories/category-type';
import { tk } from '@/i18n/translations';
import { AppField } from '@/shared/presentation/AppField';
import { AppPicker } from '@/shared/presentation/components/AppPicker';
import { AppIcons } from '@/shared/presentation/icons';

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

  return (
    <AppField label={t(tk.purchase.basicSection.category)} icon={AppIcons.category} error={error}>
      <AppPicker
        options={filteredCategories}
        selectedOption={isSelectedCategoryInFiltered ? selectedCategory : filteredCategories[0]}
        onSelectOption={onSelectCategory}
        onOptionLabel={(category) => category.name}
      />
    </AppField>
  );
}
