import { Stack } from 'expo-router';
import React from 'react';
import { useEditAccountForm } from '@/accounts/presentation/core/use-edit-account-form';
import { EditAccountFormContent } from '@/accounts/presentation/EditAccountFormContent';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useFormActions } from '@/shared/presentation/core/use-form-actions';

export const EditAccountForm = () => {
  const { saveForm, isDirty, errors } = useEditAccountForm();
  const { handleSave, handleCancel } = useFormActions({ saveForm, isDirty, errors });

  return (
    <>
      <Stack.Toolbar placement="right">
        <Stack.Toolbar.Button icon={AppIcon.select(AppIconMap.save)} onPress={handleSave} />
      </Stack.Toolbar>
      <Stack.Toolbar placement="left">
        <Stack.Toolbar.Button icon={AppIcon.select(AppIconMap.back)} onPress={handleCancel} />
      </Stack.Toolbar>
      <EditAccountFormContent />
    </>
  );
};
