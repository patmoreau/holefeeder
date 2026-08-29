import React from 'react';
import { EditAccountFormData } from '@/accounts/presentation/core/edit-account-form-data';
import { EditAccountFormProvider, validateEditAccountForm } from '@/accounts/presentation/core/use-edit-account-form';
import { EditAccountForm } from '@/accounts/presentation/EditAccountForm';
import { AppScreen } from '@/shared/presentation/AppScreen';

// A null id is what the shared save reads as "this account does not exist yet", so the
// same form that edits an account creates this one.
export const AddAccountScreen = () => (
  <AppScreen testID="add-account-screen">
    <EditAccountFormProvider initialValue={EditAccountFormData.forNewAccount()} validate={validateEditAccountForm} validateOnChange>
      <EditAccountForm />
    </EditAccountFormProvider>
  </AppScreen>
);
