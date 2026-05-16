import { Stack } from 'expo-router';
import React from 'react';
import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { Tag } from '@/flows/core/flows/tag';
import { useFlowForm } from '@/flows/presentation/flow/core/use-flow-form';
import { FlowFormContent } from '@/flows/presentation/flow/FlowFormContent';
import { useFormActions } from '@/shared/presentation/core/use-form-actions';
import { AppIcons } from '@/shared/presentation/icons';

interface FlowFormProps {
  accounts: Account[];
  categories: Category[];
  tags: Tag[];
}

export const FlowForm = ({ accounts, categories, tags }: FlowFormProps) => {
  const { saveForm, isDirty, errors } = useFlowForm();
  const { handleSave, handleCancel } = useFormActions({ saveForm, isDirty, errors });

  return (
    <>
      <Stack.Toolbar placement="right">
        <Stack.Toolbar.Button icon={AppIcons.save} onPress={handleSave} />
      </Stack.Toolbar>
      <Stack.Toolbar placement="left">
        <Stack.Toolbar.Button icon={AppIcons.back} onPress={handleCancel} />
      </Stack.Toolbar>

      <FlowFormContent accounts={accounts} categories={categories} tags={tags} />
    </>
  );
};
