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
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useOnboardingFirstAccount } from '@/user-registration/presentation/core/use-onboarding-first-account';

// See BudgetPeriod for why the action is a row rather than a toolbar button.
const FirstAccountStep = () => {
  const { isSaving, finish } = useOnboardingFirstAccount();
  const { t } = useTranslation();

  return (
    <EditAccountFormContent
      footer={
        <AppFieldSection>
          <AppField icon={AppIconMap.save}>
            <AppButton
              label={t(tk.onboarding.firstAccountFinish)}
              variant="link"
              disabled={isSaving}
              onPress={finish}
              testID="onboarding-first-account-finish-button"
            />
          </AppField>
        </AppFieldSection>
      }
    />
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
