import { Stack } from 'expo-router';
import React from 'react';
import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { Tag } from '@/flows/core/flows/tag';
import { usePurchaseForm } from '@/flows/presentation/purchase/core/use-purchase-form';
import { PurchaseFormContent } from '@/flows/presentation/purchase/PurchaseFormContent';
import { useFormActions } from '@/shared/presentation/core/use-form-actions';
import { AppIcons } from '@/shared/presentation/icons';

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
        <Stack.Toolbar.Button icon={AppIcons.save} onPress={handleSave} />
      </Stack.Toolbar>
      <Stack.Toolbar placement="left">
        <Stack.Toolbar.Button icon={AppIcons.back} onPress={handleCancel} />
      </Stack.Toolbar>

      <PurchaseFormContent accounts={accounts} categories={categories} tags={tags} />
    </>
  );
};
