import React, { useEffect, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { CategoryTypes } from '@/flows/core/categories/category-type';
import { Tag } from '@/flows/core/flows/tag';
import { FlowType } from '@/flows/presentation/flow/core/flow-form-data';
import { useFlowForm } from '@/flows/presentation/flow/core/use-flow-form';
import { FlowTypeSection } from '@/flows/presentation/flow/FlowTypeSection';
import { AccountField } from '@/flows/presentation/shared/components/AccountField';
import { AmountField, AmountFieldRef } from '@/flows/presentation/shared/components/AmountField';
import { CategoryField } from '@/flows/presentation/shared/components/CategoryField';
import { DescriptionField } from '@/flows/presentation/shared/components/DescriptionField';
import { TagList } from '@/flows/presentation/shared/components/TagList';
import { tk } from '@/i18n/translations';
import { AppForm } from '@/shared/presentation/AppForm';
import { AppSection } from '@/shared/presentation/AppSection';
import { DateField } from '@/shared/presentation/fields/DateField';

type FlowFormProps = {
  accounts: Account[];
  categories: Category[];
  tags: Tag[];
};

export const FlowFormContent = ({ accounts, categories, tags }: FlowFormProps) => {
  const { t } = useTranslation();
  const { formData, updateFormField } = useFlowForm();
  const amountFieldRef = useRef<AmountFieldRef>(null);

  const updateAccount = (account: Account) => updateFormField('account', account);

  const updateCategory = (category: Category) => updateFormField('category', category);

  const selectedTags = formData?.tags ?? [];
  const updateTags = (next: Tag[]) => updateFormField('tags', next);
  const updateDescription = (value: string) => updateFormField('description', value);

  const variant = formData.flowType === FlowType.expense ? CategoryTypes.expense : CategoryTypes.gain;

  useEffect(() => {
    // Auto-focus the amount field when the component mounts
    amountFieldRef.current?.focus();
  }, []);

  return (
    <AppForm>
      <FlowTypeSection selectedFlowType={formData.flowType} onSelectFlowType={(type) => updateFormField('flowType', type)} />
      <AmountField ref={amountFieldRef} amount={formData.amount} onAmountChange={(amount) => updateFormField('amount', amount)} />

      <AppSection>
        <DateField
          label={t(tk.purchase.basicSection.date)}
          selectedDate={formData.date}
          onDateSelected={(date) => updateFormField('date', date)}
        />
        <AccountField
          label={t(tk.purchase.basicSection.account)}
          accounts={accounts}
          selectedAccount={formData.account}
          onSelectAccount={updateAccount}
        />
        <CategoryField categories={categories} selectedCategory={formData.category} onSelectCategory={updateCategory} variant={variant} />
        <TagList tags={tags} selected={selectedTags} onChange={updateTags} categoryId={formData.category.id} />
        <DescriptionField description={formData.description} onDescriptionChange={updateDescription} />
      </AppSection>
    </AppForm>
  );
};
