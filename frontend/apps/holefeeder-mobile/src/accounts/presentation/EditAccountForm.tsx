import React from 'react';
import { useTranslation } from 'react-i18next';
import { useEditAccountForm } from '@/accounts/presentation/core/use-edit-account-form';
import { EditAccountFormContent } from '@/accounts/presentation/EditAccountFormContent';
import { tk } from '@/i18n/translations';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppToolbar } from '@/shared/presentation/components/native/AppToolbar';
import { AppToolbarButton } from '@/shared/presentation/components/native/AppToolbarButton';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useFormActions } from '@/shared/presentation/core/use-form-actions';

export const EditAccountForm = () => {
  const { t } = useTranslation();
  const { saveForm, isDirty, errors } = useEditAccountForm();
  const { handleSave, handleCancel } = useFormActions({ saveForm, isDirty, errors });

  return (
    <>
      <AppToolbar placement="right">
        <AppToolbarButton
          icon={AppIcon.select(AppIconMap.save)}
          testID="edit-account-save-button"
          accessibilityLabel={t(tk.common.save)}
          onPress={handleSave}
        />
      </AppToolbar>
      <AppToolbar placement="left">
        <AppToolbarButton
          icon={AppIcon.select(AppIconMap.back)}
          testID="edit-account-back-button"
          accessibilityLabel={t(tk.common.back)}
          onPress={handleCancel}
        />
      </AppToolbar>
      <EditAccountFormContent />
    </>
  );
};
