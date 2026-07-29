import { useTranslation } from 'react-i18next';
import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { Tag } from '@/flows/core/flows/tag';
import { FlowType } from '@/flows/presentation/flow/core/flow-form-data';
import { useFlowForm } from '@/flows/presentation/flow/core/use-flow-form';
import { FlowTypeSection } from '@/flows/presentation/flow/FlowTypeSection';
import { AccountField } from '@/flows/presentation/shared/components/AccountField';
import { CategoryField } from '@/flows/presentation/shared/components/CategoryField';
import { TagList } from '@/flows/presentation/shared/components/TagList';
import { amountToneFor } from '@/flows/presentation/shared/core/amount-tone';
import { tk } from '@/i18n/translations';
import { CategoryTypes } from '@/shared/core/category-type';
import { AmountField } from '@/shared/presentation/components/fields/AmountField';
import { DateField } from '@/shared/presentation/components/fields/DateField';
import { DescriptionField } from '@/shared/presentation/components/fields/DescriptionField';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppFieldGroup } from '@/shared/presentation/components/native/AppFieldGroup';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppNative } from '@/shared/presentation/components/native/AppNative';

type FlowFormProps = {
  accounts: Account[];
  categories: Category[];
  tags: Tag[];
};

export const FlowFormContent = ({ accounts, categories, tags }: FlowFormProps) => {
  const { t } = useTranslation();
  const { formData, updateFormField } = useFlowForm();

  const updateAccount = (account: Account) => updateFormField('account', account);

  const updateCategory = (category: Category) => updateFormField('category', category);

  const selectedTags = formData?.tags ?? [];
  const updateTags = (next: Tag[]) => updateFormField('tags', next);
  const updateDescription = (value: string) => updateFormField('description', value);

  const variant = formData.flowType === FlowType.expense ? CategoryTypes.expense : CategoryTypes.gain;

  return (
    <AppNative style={{ flex: 1 }}>
      <AppColumn spacing={8}>
        <FlowTypeSection selectedFlowType={formData.flowType} onSelectFlowType={(type) => updateFormField('flowType', type)} />
        <AmountField
          autoFocus
          amount={formData.amount}
          onAmountChange={(amount) => updateFormField('amount', amount)}
          tone={amountToneFor(formData.flowType)}
        />
        <AppFieldGroup>
          <AppFieldSection>
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
          </AppFieldSection>
        </AppFieldGroup>
      </AppColumn>
    </AppNative>
  );
};
