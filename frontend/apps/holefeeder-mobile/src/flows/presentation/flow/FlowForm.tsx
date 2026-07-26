import { Stack } from 'expo-router';
import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { Tag } from '@/flows/core/flows/tag';
import { useFlowForm } from '@/flows/presentation/flow/core/use-flow-form';
import { FlowFormContent } from '@/flows/presentation/flow/FlowFormContent';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useFormActions } from '@/shared/presentation/core/use-form-actions';

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
        <Stack.Toolbar.Button icon={AppIcon.select(AppIconMap.save)} onPress={handleSave} />
      </Stack.Toolbar>
      <Stack.Toolbar placement="left">
        <Stack.Toolbar.Button icon={AppIcon.select(AppIconMap.back)} onPress={handleCancel} />
      </Stack.Toolbar>
      <FlowFormContent accounts={accounts} categories={categories} tags={tags} />
    </>
  );
};
