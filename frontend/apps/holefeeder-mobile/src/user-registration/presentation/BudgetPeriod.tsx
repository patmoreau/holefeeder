import { Stack } from 'expo-router';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { SettingsFormData } from '@/settings/core/settings-form-data';
import { BudgetSettingsFormContent } from '@/settings/presentation/budget/BudgetSettingsFormContent';
import { SettingsFormProvider, validateSettingsForm } from '@/settings/presentation/core/use-settings-form';
import { DefaultSettings } from '@/shared/core/settings';
import { AppScreen } from '@/shared/presentation/AppScreen';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useOnboardingBudgetPeriod } from '@/user-registration/presentation/core/use-onboarding-budget-period';

// Inside the provider so it can reach the form it is saving. The action is a row in
// the form itself: rows are laid out and hit-tested reliably, and unlike the header
// toolbar they carry a testID for the flows to select.
const BudgetPeriodStep = () => {
  const { isSaving, finish } = useOnboardingBudgetPeriod();
  const { t } = useTranslation();

  return (
    <BudgetSettingsFormContent
      footer={
        <AppFieldSection>
          <AppField icon={AppIconMap.save}>
            <AppButton
              label={t(tk.onboarding.budgetPeriodContinue)}
              variant="link"
              disabled={isSaving}
              onPress={finish}
              testID="onboarding-budget-period-continue-button"
            />
          </AppField>
        </AppFieldSection>
      }
    />
  );
};

// The defaults seed the form rather than standing in for an answer: whatever the
// caller leaves here is written to store_items, so the app stops running on values
// nobody ever chose.
const BudgetPeriodScreen = () => {
  const { t } = useTranslation();

  const initialData: SettingsFormData = {
    effectiveDate: DefaultSettings.effectiveDate,
    frequency: DefaultSettings.frequency,
    intervalType: DefaultSettings.intervalType,
  };

  return (
    <AppScreen testID="onboarding-budget-period-screen">
      <Stack.Screen options={{ title: t(tk.onboarding.budgetPeriodTitle) }} />
      <SettingsFormProvider initialValue={initialData} validate={validateSettingsForm} validateOnChange>
        <BudgetPeriodStep />
      </SettingsFormProvider>
    </AppScreen>
  );
};

export default BudgetPeriodScreen;
