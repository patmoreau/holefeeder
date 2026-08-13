import { router } from 'expo-router';
import { useState } from 'react';
import { useEditAccountForm } from '@/accounts/presentation/core/use-edit-account-form';

export type OnboardingFirstAccountState = {
  isSaving: boolean;
  finish: () => Promise<void>;
};

// Hands over to the suggested categories, which is the step that opens the gate.
export const useOnboardingFirstAccount = (): OnboardingFirstAccountState => {
  const { saveForm } = useEditAccountForm();
  const [isSaving, setIsSaving] = useState(false);

  const finish = async () => {
    setIsSaving(true);
    try {
      const saved = await saveForm();
      if (!saved) {
        // The form surfaces its own errors; staying put is the whole response.
        return;
      }
      // replace, not push: the account is saved and going back would re-ask it.
      router.replace('/Categories');
    } finally {
      setIsSaving(false);
    }
  };

  return {
    isSaving: isSaving,
    finish: finish,
  };
};
