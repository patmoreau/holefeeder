import { Stack } from 'expo-router';
import React from 'react';
import { BudgetSettingsFormContent } from '@/settings/presentation/BudgetSettingsFormContent';
import { useSettingsForm } from '@/settings/presentation/core/use-settings-form';
import { useFormActions } from '@/shared/presentation/core/use-form-actions';
import { AppIcons } from '@/shared/presentation/icons';

export const BudgetSettingsForm = () => {
  const { saveForm, isDirty, errors } = useSettingsForm();
  const { handleSave, handleCancel } = useFormActions({ saveForm, isDirty, errors });

  return (
    <>
      <Stack.Toolbar placement="right">
        <Stack.Toolbar.Button icon={AppIcons.save} onPress={handleSave} />
      </Stack.Toolbar>
      <Stack.Toolbar placement="left">
        <Stack.Toolbar.Button icon={AppIcons.back} onPress={handleCancel} />
      </Stack.Toolbar>

      <BudgetSettingsFormContent />
    </>
  );
};
