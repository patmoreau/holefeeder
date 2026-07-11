import { Icon } from '@expo/ui';
import { Stack } from 'expo-router';
import { CategoryFormContent } from '@/flows/presentation/categories/CategoryFormContent';
import { useCategoryForm } from '@/flows/presentation/categories/core/use-category-form';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useFormActions } from '@/shared/presentation/core/use-form-actions';

export const CategoryForm = () => {
  const { saveForm, isDirty, errors } = useCategoryForm();
  const { handleSave, handleCancel } = useFormActions({ saveForm, isDirty, errors });

  return (
    <>
      <Stack.Toolbar placement="right">
        <Stack.Toolbar.Button icon={Icon.select(AppIconMap.save)} onPress={handleSave} />
      </Stack.Toolbar>
      <Stack.Toolbar placement="left">
        <Stack.Toolbar.Button icon={Icon.select(AppIconMap.back)} onPress={handleCancel} />
      </Stack.Toolbar>
      <CategoryFormContent />
    </>
  );
};
