import { useState } from 'react';
import { useEditAccountForm } from '@/accounts/presentation/core/use-edit-account-form';
import { useRegistration } from '@/user-registration/presentation/RegistrationProvider';

export type OnboardingFirstAccountState = {
  isSaving: boolean;
  finish: () => Promise<void>;
};

// The last step of onboarding, so this is the one that opens the gate: rechecking
// makes the status registered and the caller lands in the app.
export const useOnboardingFirstAccount = (): OnboardingFirstAccountState => {
  const { saveForm } = useEditAccountForm();
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
