import { useTranslation } from 'react-i18next';
import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { Tag } from '@/flows/core/flows/tag';
import { useCashflowForm } from '@/flows/presentation/cashflows/core/use-cashflow-form';
import { AccountField } from '@/flows/presentation/shared/components/AccountField';
import { CategoryField } from '@/flows/presentation/shared/components/CategoryField';
import { TagList } from '@/flows/presentation/shared/components/TagList';
import { tk } from '@/i18n/translations';
import { AmountField } from '@/shared/presentation/components/fields/AmountField';
import { DateField } from '@/shared/presentation/components/fields/DateField';
import { DateIntervalTypeField } from '@/shared/presentation/components/fields/DateIntervalTypeField';
import { DescriptionField } from '@/shared/presentation/components/fields/DescriptionField';
import { FrequencyField } from '@/shared/presentation/components/fields/FrequencyField';
import { RecurrenceField } from '@/shared/presentation/components/fields/RecurrenceField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppForm } from '@/shared/presentation/components/native/AppForm';

type Props = {
  accounts: Account[];
  categories: Category[];
  tags: Tag[];
};

export const CashflowFormContent = ({ accounts, categories, tags }: Props) => {
  const { t } = useTranslation();
  const { formData, updateFormField, errors } = useCashflowForm();

  return (
    <AppForm>
      <AmountField amount={formData.amount} onAmountChange={(amount) => updateFormField('amount', amount)} />
      <AppFieldSection>
        <AccountField
          label={t(tk.purchase.basicSection.account)}
          accounts={accounts}
          selectedAccount={formData.account}
          onSelectAccount={(account) => updateFormField('account', account)}
          error={errors.account}
        />
        <CategoryField
          categories={categories}
          selectedCategory={formData.category}
          onSelectCategory={(category) => updateFormField('category', category)}
          variant={formData.category.type}
          error={errors.category}
        />
        <TagList tags={tags} selected={formData.tags} onChange={(next) => updateFormField('tags', next)} categoryId={formData.category.id} />
        <DescriptionField description={formData.description} onDescriptionChange={(value) => updateFormField('description', value)} />
      </AppFieldSection>
      <AppFieldSection title={t(tk.purchase.cashflowSection.title)}>
        <DateField
          label={t(tk.purchase.cashflowSection.date)}
          selectedDate={formData.effectiveDate}
          onDateSelected={(date) => updateFormField('effectiveDate', date)}
        />
        <DateIntervalTypeField
          selectedDateIntervalType={formData.intervalType}
          onSelectDateIntervalType={(type) => updateFormField('intervalType', type)}
        />
        <FrequencyField selectedFrequency={formData.frequency} onSelectFrequency={(frequency) => updateFormField('frequency', frequency)} />
        <RecurrenceField
          selectedRecurrence={formData.recurrence}
          onSelectRecurrence={(recurrence) => updateFormField('recurrence', recurrence)}
        />
      </AppFieldSection>
    </AppForm>
  );
};
