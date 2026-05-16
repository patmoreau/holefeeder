import React, { useEffect, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { usePayUpcomingForm } from '@/flows/presentation/pay-upcoming/core/use-pay-upcoming-form';
import { AmountField, AmountFieldRef } from '@/flows/presentation/shared/components/AmountField';
import { tk } from '@/i18n/translations';
import { AppSection } from '@/shared/presentation/AppSection';
import { DateField } from '@/shared/presentation/fields/DateField';

export const PayUpcomingFormContent = () => {
  const { t } = useTranslation();
  const { formData, updateFormField } = usePayUpcomingForm();
  const amountFieldRef = useRef<AmountFieldRef>(null);

  useEffect(() => {
    amountFieldRef.current?.focus();
  }, []);

  return (
    <>
      <AmountField ref={amountFieldRef} amount={formData.amount} onAmountChange={(amount) => updateFormField('amount', amount)} />
      <AppSection>
        <DateField
          label={t(tk.purchase.basicSection.date)}
          selectedDate={formData.date}
          onDateSelected={(date) => updateFormField('date', date)}
        />
      </AppSection>
    </>
  );
};
