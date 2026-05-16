import { Stack } from 'expo-router';
import React from 'react';
import { useEditAccountForm } from '@/accounts/presentation/core/use-edit-account-form';
import { EditAccountFormContent } from '@/accounts/presentation/EditAccountFormContent';
import { useFormActions } from '@/shared/presentation/core/use-form-actions';
import { AppIcons } from '@/shared/presentation/icons';

export const EditAccountForm = () => {
  const { saveForm, isDirty, errors } = useEditAccountForm();
  const { handleSave, handleCancel } = useFormActions({ saveForm, isDirty, errors });

  return (
    <>
      <Stack.Toolbar placement="right">
        <Stack.Toolbar.Button icon={AppIcons.save} onPress={handleSave} />
      </Stack.Toolbar>
      <Stack.Toolbar placement="left">
        <Stack.Toolbar.Button icon={AppIcons.back} onPress={handleCancel} />
      </Stack.Toolbar>
      <EditAccountFormContent />
    </>
  );
};
