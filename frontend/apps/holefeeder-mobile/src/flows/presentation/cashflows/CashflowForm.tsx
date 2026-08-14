import { useTranslation } from 'react-i18next';
import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { Tag } from '@/flows/core/flows/tag';
import { CashflowFormContent } from '@/flows/presentation/cashflows/CashflowFormContent';
import { useCashflowForm } from '@/flows/presentation/cashflows/core/use-cashflow-form';
import { tk } from '@/i18n/translations';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppToolbar } from '@/shared/presentation/components/native/AppToolbar';
import { AppToolbarButton } from '@/shared/presentation/components/native/AppToolbarButton';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useFormActions } from '@/shared/presentation/core/use-form-actions';

type Props = {
  accounts: Account[];
  categories: Category[];
  tags: Tag[];
};

export const CashflowForm = ({ accounts, categories, tags }: Props) => {
  const { t } = useTranslation();
  const { saveForm, isDirty, errors } = useCashflowForm();
  const { handleSave, handleCancel } = useFormActions({ saveForm, isDirty, errors });

  return (
    <>
      <AppToolbar placement="right">
        <AppToolbarButton
          icon={AppIcon.select(AppIconMap.save)}
          testID="cashflow-save-button"
          accessibilityLabel={t(tk.common.save)}
          onPress={handleSave}
        />
      </AppToolbar>
      <AppToolbar placement="left">
        <AppToolbarButton
          icon={AppIcon.select(AppIconMap.back)}
          testID="cashflow-back-button"
          accessibilityLabel={t(tk.common.back)}
          onPress={handleCancel}
        />
      </AppToolbar>
      <CashflowFormContent accounts={accounts} categories={categories} tags={tags} />
    </>
  );
};
