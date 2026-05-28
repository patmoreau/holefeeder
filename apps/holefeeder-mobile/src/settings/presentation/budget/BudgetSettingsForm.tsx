import { Icon } from '@expo/ui';
import { Stack } from 'expo-router';
import { BudgetSettingsFormContent } from '@/settings/presentation/BudgetSettingsFormContent';
import { useSettingsForm } from '@/settings/presentation/core/use-settings-form';
import { AppIconMap } from '@/shared/presentation/components/app/app-icon-map';
import { useFormActions } from '@/shared/presentation/core/use-form-actions';

export const BudgetSettingsForm = () => {
  const { saveForm, isDirty, errors } = useSettingsForm();
  const { handleSave, handleCancel } = useFormActions({ saveForm, isDirty, errors });

  return (
    <>
      <Stack.Toolbar placement="right">
        <Stack.Toolbar.Button icon={Icon.select(AppIconMap.save)} onPress={handleSave} />
      </Stack.Toolbar>
      <Stack.Toolbar placement="left">
        <Stack.Toolbar.Button icon={Icon.select(AppIconMap.back)} onPress={handleCancel} />
      </Stack.Toolbar>
      <BudgetSettingsFormContent />
    </>
  );
};
