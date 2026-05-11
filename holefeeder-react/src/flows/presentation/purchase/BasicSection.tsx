import React from 'react';
import { useTranslation } from 'react-i18next';
import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { CategoryTypes } from '@/flows/core/categories/category-type';
import { Tag } from '@/flows/core/flows/tag';
import { PurchaseType } from '@/flows/presentation/purchase/core/purchase-form-data';
import { usePurchaseForm } from '@/flows/presentation/purchase/core/use-purchase-form';
import { AccountField } from '@/flows/presentation/shared/components/AccountField';
import { CategoryField } from '@/flows/presentation/shared/components/CategoryField';
import { DescriptionField } from '@/flows/presentation/shared/components/DescriptionField';
import { TagList } from '@/flows/presentation/shared/components/TagList';
import { tk } from '@/i18n/translations';
import { AppSection } from '@/shared/presentation/AppSection';
import { DateField } from '@/shared/presentation/fields/DateField';

type Props = {
  accounts: Account[];
  categories: Category[];
  tags: Tag[];
};

export function BasicSection({ accounts, categories, tags }: Props) {
  const { t } = useTranslation();
  const { formData, updateFormField } = usePurchaseForm();

  const updateSourceAccount = (account: Account) => updateFormField('sourceAccount', account);

  const updateCategory = (category: Category) => updateFormField('category', category);

  const selectedTags = formData?.tags ?? [];
  const updateTags = (next: Tag[]) => updateFormField('tags', next);
  const updateDescription = (value: string) => updateFormField('description', value);

  const variant = formData.purchaseType === PurchaseType.expense ? CategoryTypes.expense : CategoryTypes.gain;

  return (
    <AppSection>
      <DateField
        label={t(tk.purchase.basicSection.date)}
        selectedDate={formData.date}
        onDateSelected={(date) => updateFormField('date', date)}
      />
      <AccountField
        label={t(tk.purchase.basicSection.account)}
        accounts={accounts}
        selectedAccount={formData.sourceAccount}
        onSelectAccount={updateSourceAccount}
      />
      <CategoryField categories={categories} selectedCategory={formData.category} onSelectCategory={updateCategory} variant={variant} />
      <TagList tags={tags} selected={selectedTags} onChange={updateTags} categoryId={formData.category.id} />
      <DescriptionField description={formData.description} onDescriptionChange={updateDescription} />
    </AppSection>
  );
}
