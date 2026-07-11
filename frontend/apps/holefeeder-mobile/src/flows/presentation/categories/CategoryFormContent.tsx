import { useTranslation } from 'react-i18next';
import { CategoryTypes } from '@/flows/core/categories/category-type';
import { CategoryColorField } from '@/flows/presentation/categories/CategoryColorField';
import { useCategoryForm } from '@/flows/presentation/categories/core/use-category-form';
import { AmountField } from '@/flows/presentation/shared/components/AmountField';
import { tk } from '@/i18n/translations';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppForm } from '@/shared/presentation/components/native/AppForm';
import { AppPicker } from '@/shared/presentation/components/native/AppPicker';
import { AppSwitch } from '@/shared/presentation/components/native/AppSwitch';
import { AppTextInput } from '@/shared/presentation/components/native/AppTextInput';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

const categoryTypeOptions = Object.values(CategoryTypes).map((type) => ({ id: type, label: type }));

export const CategoryFormContent = () => {
  const { t } = useTranslation();
  const { formData, updateFormField, errors } = useCategoryForm();

  const selectedTypeOption = categoryTypeOptions.find((o) => o.id === formData.type) ?? categoryTypeOptions[0];

  return (
    <AppForm>
      <AppFieldSection>
        <AppField
          icon={AppIconMap.category}
          label={t(tk.categoryEdit.name)}
          error={errors.name ? t(tk.categoryEdit.errors.nameRequired) : undefined}
        >
          <AppTextInput placeholder={t(tk.categoryEdit.name)} value={formData.name} onChangeText={(value) => updateFormField('name', value)} />
        </AppField>
        <AppField icon={AppIconMap.category} label={t(tk.categoryEdit.type)}>
          <AppPicker
            options={categoryTypeOptions}
            selectedOption={selectedTypeOption}
            onSelectOption={(option) => updateFormField('type', option.id)}
            onOptionLabel={(option) => t(tk.categoryEdit.categoryTypes[option.id])}
          />
        </AppField>
        <CategoryColorField color={formData.color} onColorChange={(value) => updateFormField('color', value)} />
      </AppFieldSection>
      <AppFieldSection>
        <AmountField amount={formData.budgetAmount} onAmountChange={(value) => updateFormField('budgetAmount', value)} />
      </AppFieldSection>
      <AppFieldSection>
        <AppField icon={AppIconMap.add} label={t(tk.categoryEdit.favorite)}>
          <AppSwitch value={formData.favorite} onChange={(value) => updateFormField('favorite', value)} />
        </AppField>
      </AppFieldSection>
    </AppForm>
  );
};
