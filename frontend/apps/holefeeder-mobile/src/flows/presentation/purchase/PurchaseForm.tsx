import { Stack } from 'expo-router';
import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { Tag } from '@/flows/core/flows/tag';
import { usePurchaseForm } from '@/flows/presentation/purchase/core/use-purchase-form';
import { PurchaseFormContent } from '@/flows/presentation/purchase/PurchaseFormContent';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useFormActions } from '@/shared/presentation/core/use-form-actions';

interface PurchaseFormProps {
  accounts: Account[];
  categories: Category[];
  tags: Tag[];
}

export const PurchaseForm = ({ accounts, categories, tags }: PurchaseFormProps) => {
  const { saveForm, isDirty, errors } = usePurchaseForm();
  const { handleSave, handleCancel } = useFormActions({ saveForm, isDirty, errors });

  return (
    <>
      <Stack.Toolbar placement="right">
        <Stack.Toolbar.Button icon={AppIcon.select(AppIconMap.save)} onPress={handleSave} />
      </Stack.Toolbar>
      <Stack.Toolbar placement="left">
        <Stack.Toolbar.Button icon={AppIcon.select(AppIconMap.back)} onPress={handleCancel} />
      </Stack.Toolbar>
      <PurchaseFormContent accounts={accounts} categories={categories} tags={tags} />
    </>
  );
};
