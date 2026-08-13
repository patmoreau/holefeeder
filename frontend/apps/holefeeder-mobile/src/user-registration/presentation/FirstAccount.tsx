import { DateOnly, today } from '@holefeeder/shared/core';
import { Stack } from 'expo-router';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { AccountTypes } from '@/accounts/core/account-type';
import { EditAccountFormData } from '@/accounts/presentation/core/edit-account-form-data';
import { EditAccountFormProvider, validateEditAccountForm } from '@/accounts/presentation/core/use-edit-account-form';
import { EditAccountFormContent } from '@/accounts/presentation/EditAccountFormContent';
import { tk } from '@/i18n/translations';
import { AppScreen } from '@/shared/presentation/AppScreen';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useOnboardingFirstAccount } from '@/user-registration/presentation/core/use-onboarding-first-account';

// Inside the provider so it can reach the form it is saving. Save lives in the header
// toolbar for the same reason as the budget step: a button after the form is clipped
// past the home indicator.
const FirstAccountStep = () => {
  const { isSaving, finish } = useOnboardingFirstAccount();

  return (
    <>
      <Stack.Toolbar placement="right">
        {/* No testID: Stack.Toolbar.Button does not accept one, which is why the
            Maestro flow has to tap this by position. */}
        <Stack.Toolbar.Button icon={AppIcon.select(AppIconMap.save)} onPress={finish} disabled={isSaving} />
      </Stack.Toolbar>
      <EditAccountFormContent />
    </>
  );
};

// A null id is what the shared save reads as "this account does not exist yet", so
// the same form that edits an account creates this one.
const FirstAccountScreen = () => {
  const { t } = useTranslation();

  const initialData: EditAccountFormData = {
    id: null,
    name: '',
    type: AccountTypes.checking,
    openBalance: 0,
    openDate: DateOnly.valid(today()),
    description: '',
    favorite: true,
    inactive: false,
  };

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
