import { Logger } from '@holefeeder/shared/core';
import { useCallback } from 'react';
import { useTranslation } from 'react-i18next';
import { endEditing } from '@/shared/presentation/core/end-editing';
import { goBack } from '@/shared/presentation/core/navigation';
import { showAlert } from '@/shared/presentation/core/show-alert';

const logger = Logger.create('useFormActions');

export type FormContext = {
  saveForm: () => Promise<boolean>;
  isDirty: boolean;
  errors: Record<string, unknown>;
};

type FormActions = {
  handleSave: () => Promise<void>;
  handleCancel: () => Promise<void>;
};

const leaveForm = async () => {
  await endEditing();
  goBack();
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
    await endEditing();
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

  const handleCancel = useCallback(async () => {
    if (!isDirty) {
      await leaveForm();
      return;
    }
    showDiscardAlert({ onConfirm: leaveForm });
  }, [isDirty, showDiscardAlert]);

  return { handleSave, handleCancel };
};
