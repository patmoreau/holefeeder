import { createLogger } from '@powersync/common';
import { useCallback } from 'react';
import { useTranslation } from 'react-i18next';
import { goBack } from '@/shared/presentation/navigation';
import { showAlert } from '@/shared/presentation/show-alert';

const logger = createLogger('useFormActions');

export type FormContext = {
  saveForm: () => Promise<boolean>;
  isDirty: boolean;
  errors: Record<string, unknown>;
};

type FormActions = {
  handleSave: () => Promise<void>;
  handleCancel: () => void;
};

/**
 * Reusable hook for common form save/cancel actions.
 * Handles validation error display, dirty state checks, and navigation.
 */
export const useFormActions = ({ saveForm, isDirty, errors }: FormContext): FormActions => {
  const { t } = useTranslation();
  const { showDiscardAlert, showFormErrorAlert } = showAlert(t);

  const handleSave = useCallback(async () => {
    logger.warn(`handleSave called. isDirty: ${isDirty}, errors: ${JSON.stringify(errors)}`);
    if (isDirty) {
      const success = await saveForm();
      if (!success) {
        // Validation failed or save failed
        const errorCount = Object.keys(errors).length;
        if (errorCount > 0) {
          showFormErrorAlert({ errors: errors as Record<string, string>, errorCount });
        }
        // If save failed (not validation), error sheet will be shown automatically
        return;
      }
    }
    goBack();
  }, [errors, isDirty, saveForm, showFormErrorAlert]);

  const handleCancel = useCallback(() => {
    if (!isDirty) {
      goBack();
      return;
    }
    showDiscardAlert({ onConfirm: goBack });
  }, [isDirty, showDiscardAlert]);

  return { handleSave, handleCancel };
};
