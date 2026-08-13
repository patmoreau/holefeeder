import { router } from 'expo-router';
import { useState } from 'react';
import { useSettingsForm } from '@/settings/presentation/core/use-settings-form';

export type OnboardingBudgetPeriodState = {
  isSaving: boolean;
  finish: () => Promise<void>;
};

// Hands over to the first account step, which is the one that opens the gate.
export const useOnboardingBudgetPeriod = (): OnboardingBudgetPeriodState => {
  const { saveForm } = useSettingsForm();
  const [isSaving, setIsSaving] = useState(false);

  const finish = async () => {
    setIsSaving(true);
    try {
      const saved = await saveForm();
      if (!saved) {
        // The form surfaces its own errors; staying put is the whole response.
        return;
      }
      // replace, not push: the period is saved and going back would re-ask it.
      router.replace('/FirstAccount');
    } finally {
      setIsSaving(false);
    }
  };

  return {
    isSaving: isSaving,
    finish: finish,
  };
};
