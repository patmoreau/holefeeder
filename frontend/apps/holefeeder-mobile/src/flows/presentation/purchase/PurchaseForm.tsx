import { useTranslation } from 'react-i18next';
import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { Tag } from '@/flows/core/flows/tag';
import { usePurchaseForm } from '@/flows/presentation/purchase/core/use-purchase-form';
import { PurchaseFormContent } from '@/flows/presentation/purchase/PurchaseFormContent';
import { tk } from '@/i18n/translations';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppToolbar } from '@/shared/presentation/components/native/AppToolbar';
import { AppToolbarButton } from '@/shared/presentation/components/native/AppToolbarButton';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useFormActions } from '@/shared/presentation/core/use-form-actions';

interface PurchaseFormProps {
  accounts: Account[];
  categories: Category[];
  tags: Tag[];
}

export const PurchaseForm = ({ accounts, categories, tags }: PurchaseFormProps) => {
  const { t } = useTranslation();
  const { saveForm, isDirty, errors } = usePurchaseForm();
  const { handleSave, handleCancel } = useFormActions({ saveForm, isDirty, errors });

  return (
    <>
      <AppToolbar placement="right">
        <AppToolbarButton
          icon={AppIcon.select(AppIconMap.save)}
          testID="purchase-save-button"
          accessibilityLabel={t(tk.common.save)}
          onPress={handleSave}
        />
      </AppToolbar>
      <AppToolbar placement="left">
        <AppToolbarButton
          icon={AppIcon.select(AppIconMap.back)}
          testID="purchase-back-button"
          accessibilityLabel={t(tk.common.back)}
          onPress={handleCancel}
        />
      </AppToolbar>
      <PurchaseFormContent accounts={accounts} categories={categories} tags={tags} />
    </>
  );
};
