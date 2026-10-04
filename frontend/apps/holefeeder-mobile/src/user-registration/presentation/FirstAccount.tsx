import { Stack } from 'expo-router';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { EditAccountFormData } from '@/accounts/presentation/core/edit-account-form-data';
import { EditAccountFormProvider, validateEditAccountForm } from '@/accounts/presentation/core/use-edit-account-form';
import { EditAccountFormContent } from '@/accounts/presentation/EditAccountFormContent';
import { tk } from '@/i18n/translations';
import { AppScreen } from '@/shared/presentation/AppScreen';
import { AppPinnedActions } from '@/shared/presentation/components/native/AppPinnedActions';
import { useOnboardingFirstAccount } from '@/user-registration/presentation/core/use-onboarding-first-account';

// See BudgetPeriod for why the action is pinned below the form. Favorite is hidden: with
// a single account it means nothing, and the account is created as a favourite anyway.
const FirstAccountStep = () => {
  const { isSaving, finish } = useOnboardingFirstAccount();
  const { t } = useTranslation();
  return (
    <>
      <EditAccountFormContent showsFavorite={false} />
      <AppPinnedActions
        primary={{ label: t(tk.onboarding.firstAccountFinish), onPress: finish, testID: 'onboarding-first-account-finish-button' }}
        disabled={isSaving}
      />
    </>
  );
};

// A null id is what the shared save reads as "this account does not exist yet", so
// the same form that edits an account creates this one.
const FirstAccountScreen = () => {
  const { t } = useTranslation();

  const initialData = EditAccountFormData.forNewAccount({ favorite: true });

  return (
    <AppScreen testID="onboarding-first-account-screen">
      <Stack.Screen options={{ title: t(tk.onboarding.firstAccountTitle) }} />
      <EditAccountFormProvider initialValue={initialData} validate={validateEditAccountForm} validateOnChange>
        <FirstAccountStep />
      </EditAccountFormProvider>
    </AppScreen>
  );
};

export default FirstAccountScreen;
