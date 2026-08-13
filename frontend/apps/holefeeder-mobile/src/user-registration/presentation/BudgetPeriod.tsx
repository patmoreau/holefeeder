import { Stack } from 'expo-router';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { SettingsFormData } from '@/settings/core/settings-form-data';
import { BudgetSettingsFormContent } from '@/settings/presentation/budget/BudgetSettingsFormContent';
import { SettingsFormProvider, validateSettingsForm } from '@/settings/presentation/core/use-settings-form';
import { DefaultSettings } from '@/shared/core/settings';
import { AppScreen } from '@/shared/presentation/AppScreen';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useOnboardingBudgetPeriod } from '@/user-registration/presentation/core/use-onboarding-budget-period';

// Inside the provider so it can reach the form it is saving. The action lives in the
// header toolbar like every other form in this app: a button placed after the form in
// a SwiftUI column lands past the home indicator, clipped and untappable.
const BudgetPeriodStep = () => {
  const { isSaving, finish } = useOnboardingBudgetPeriod();

  return (
    <>
      <Stack.Toolbar placement="right">
        {/* No testID: Stack.Toolbar.Button does not accept one, which is why the
            Maestro flow has to tap this by position. */}
        <Stack.Toolbar.Button icon={AppIcon.select(AppIconMap.save)} onPress={finish} disabled={isSaving} />
      </Stack.Toolbar>
      <BudgetSettingsFormContent />
    </>
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
