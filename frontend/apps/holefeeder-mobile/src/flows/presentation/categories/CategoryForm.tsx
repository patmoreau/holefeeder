import { Stack } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { CategoryFormContent } from '@/flows/presentation/categories/CategoryFormContent';
import { useCategoryForm } from '@/flows/presentation/categories/core/use-category-form';
import { tk } from '@/i18n/translations';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppToolbarButton } from '@/shared/presentation/components/native/AppToolbarButton';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useFormActions } from '@/shared/presentation/core/use-form-actions';

export const CategoryForm = () => {
  const { t } = useTranslation();
  const { saveForm, isDirty, errors } = useCategoryForm();
  const { handleSave, handleCancel } = useFormActions({ saveForm, isDirty, errors });

  return (
    <>
      <Stack.Toolbar placement="right">
        <AppToolbarButton icon={AppIcon.select(AppIconMap.save)} accessibilityLabel={t(tk.common.save)} onPress={handleSave} />
      </Stack.Toolbar>
      <Stack.Toolbar placement="left">
        <AppToolbarButton icon={AppIcon.select(AppIconMap.back)} accessibilityLabel={t(tk.common.back)} onPress={handleCancel} />
      </Stack.Toolbar>
      <CategoryFormContent />
    </>
  );
};
