import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { BudgetSettingsFormContent } from '@/settings/presentation/budget/BudgetSettingsFormContent';
import { useSettingsForm } from '@/settings/presentation/core/use-settings-form';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppToolbar } from '@/shared/presentation/components/native/AppToolbar';
import { AppToolbarButton } from '@/shared/presentation/components/native/AppToolbarButton';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useFormActions } from '@/shared/presentation/core/use-form-actions';

export const BudgetSettingsForm = () => {
  const { t } = useTranslation();
  const { saveForm, isDirty, errors } = useSettingsForm();
  const { handleSave, handleCancel } = useFormActions({ saveForm, isDirty, errors });

  return (
    <>
      <AppToolbar placement="right">
        <AppToolbarButton
          icon={AppIcon.select(AppIconMap.save)}
          testID="budget-settings-save-button"
          accessibilityLabel={t(tk.common.save)}
          onPress={handleSave}
        />
      </AppToolbar>
      <AppToolbar placement="left">
        <AppToolbarButton
          icon={AppIcon.select(AppIconMap.back)}
          testID="budget-settings-back-button"
          accessibilityLabel={t(tk.common.back)}
          onPress={handleCancel}
        />
      </AppToolbar>
      <BudgetSettingsFormContent />
    </>
  );
};
