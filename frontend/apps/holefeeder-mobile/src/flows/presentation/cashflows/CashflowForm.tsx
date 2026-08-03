import { Stack } from 'expo-router';
import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { Tag } from '@/flows/core/flows/tag';
import { CashflowFormContent } from '@/flows/presentation/cashflows/CashflowFormContent';
import { useCashflowForm } from '@/flows/presentation/cashflows/core/use-cashflow-form';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useFormActions } from '@/shared/presentation/core/use-form-actions';

type Props = {
  accounts: Account[];
  categories: Category[];
  tags: Tag[];
};

export const CashflowForm = ({ accounts, categories, tags }: Props) => {
  const { saveForm, isDirty, errors } = useCashflowForm();
  const { handleSave, handleCancel } = useFormActions({ saveForm, isDirty, errors });

  return (
    <>
      <Stack.Toolbar placement="right">
        <Stack.Toolbar.Button icon={AppIcon.select(AppIconMap.save)} onPress={handleSave} />
      </Stack.Toolbar>
      <Stack.Toolbar placement="left">
        <Stack.Toolbar.Button icon={AppIcon.select(AppIconMap.back)} onPress={handleCancel} />
      </Stack.Toolbar>
      <CashflowFormContent accounts={accounts} categories={categories} tags={tags} />
    </>
  );
};
