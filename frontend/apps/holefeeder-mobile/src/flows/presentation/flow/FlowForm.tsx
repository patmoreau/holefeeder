import { useTranslation } from 'react-i18next';
import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { Tag } from '@/flows/core/flows/tag';
import { useFlowForm } from '@/flows/presentation/flow/core/use-flow-form';
import { FlowFormContent } from '@/flows/presentation/flow/FlowFormContent';
import { tk } from '@/i18n/translations';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppToolbar } from '@/shared/presentation/components/native/AppToolbar';
import { AppToolbarButton } from '@/shared/presentation/components/native/AppToolbarButton';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useFormActions } from '@/shared/presentation/core/use-form-actions';

interface FlowFormProps {
  accounts: Account[];
  categories: Category[];
  tags: Tag[];
}

export const FlowForm = ({ accounts, categories, tags }: FlowFormProps) => {
  const { t } = useTranslation();
  const { saveForm, isDirty, errors } = useFlowForm();
  const { handleSave, handleCancel } = useFormActions({ saveForm, isDirty, errors });

  return (
    <>
      <AppToolbar placement="right">
        <AppToolbarButton
          icon={AppIcon.select(AppIconMap.save)}
          testID="flow-save-button"
          accessibilityLabel={t(tk.common.save)}
          onPress={handleSave}
        />
      </AppToolbar>
      <AppToolbar placement="left">
        <AppToolbarButton
          icon={AppIcon.select(AppIconMap.back)}
          testID="flow-back-button"
          accessibilityLabel={t(tk.common.back)}
          onPress={handleCancel}
        />
      </AppToolbar>
      <FlowFormContent accounts={accounts} categories={categories} tags={tags} />
    </>
  );
};
