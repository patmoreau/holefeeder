import { useTranslation } from 'react-i18next';
import { UpdateRecurringAmountField } from '@/flows/presentation/pay-upcoming/components/UpdateRecurringAmountField';
import { usePayUpcomingForm } from '@/flows/presentation/pay-upcoming/core/use-pay-upcoming-form';
import { tk } from '@/i18n/translations';
import { DateField } from '@/shared/presentation/components/fields/DateField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';

export const PayUpcomingFormContent = () => {
  const { t } = useTranslation();
  const { formData, updateFormField } = usePayUpcomingForm();
  const amountChanged = formData.amount !== formData.originalAmount;

  return (
    <>
      <AppFieldSection>
        <DateField
          label={t(tk.purchase.basicSection.date)}
          selectedDate={formData.date}
          onDateSelected={(date) => updateFormField('date', date)}
        />
      </AppFieldSection>
      {amountChanged && (
        <AppFieldSection>
          <UpdateRecurringAmountField
            amount={formData.amount}
            value={formData.updateRecurringAmount}
            onChange={(value) => updateFormField('updateRecurringAmount', value)}
          />
        </AppFieldSection>
      )}
    </>
  );
};
