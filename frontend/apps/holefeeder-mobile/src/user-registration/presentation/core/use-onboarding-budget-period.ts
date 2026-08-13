import { useState } from 'react';
import { useSettingsForm } from '@/settings/presentation/core/use-settings-form';
import { useRegistration } from '@/user-registration/presentation/RegistrationProvider';

export type OnboardingBudgetPeriodState = {
  isSaving: boolean;
  finish: () => Promise<void>;
};

// Last step of onboarding for now, so it is the one that opens the gate: rechecking
// makes the status registered, and the caller lands in the app. B12 inserts the
// first account before this and takes that job over.
export const useOnboardingBudgetPeriod = (): OnboardingBudgetPeriodState => {
  const { saveForm } = useSettingsForm();
  const { recheck } = useRegistration();
  const [isSaving, setIsSaving] = useState(false);

  const finish = async () => {
    setIsSaving(true);
    try {
      const saved = await saveForm();
      if (!saved) {
        // The form surfaces its own errors; staying put is the whole response.
        return;
      }
      recheck();
    } finally {
      setIsSaving(false);
    }
  };

  return {
    isSaving: isSaving,
    finish: finish,
  };
};
