import React from 'react';
import { useTranslation } from 'react-i18next';
import { HasCashflowField } from '@/flows/presentation/purchase/components/fields/HasCashflowField';
import { usePurchaseForm } from '@/flows/presentation/purchase/core/use-purchase-form';
import { tk } from '@/i18n/translations';
import { AppSection } from '@/shared/presentation/AppSection';
import { DateField } from '@/shared/presentation/fields/DateField';
import { DateIntervalTypeField } from '@/shared/presentation/fields/DateIntervalTypeField';
import { FrequencyField } from '@/shared/presentation/fields/FrequencyField';

export const CashflowSection = () => {
  const { t } = useTranslation();
  const { formData, updateFormField } = usePurchaseForm();

  if (!formData.hasCashflow) {
    return (
      <AppSection title={t(tk.purchase.cashflowSection.title)}>
        <HasCashflowField hasCashflow={formData.hasCashflow} onHasCashflowChange={(value) => updateFormField('hasCashflow', value)} />
      </AppSection>
    );
  }

  return (
    <AppSection title={t(tk.purchase.cashflowSection.title)}>
      <HasCashflowField hasCashflow={formData.hasCashflow} onHasCashflowChange={(value) => updateFormField('hasCashflow', value)} />
      <DateField
        label={t(tk.purchase.cashflowSection.date)}
        selectedDate={formData.cashflowEffectiveDate}
        onDateSelected={(date) => updateFormField('cashflowEffectiveDate', date)}
      />
      <DateIntervalTypeField
        selectedDateIntervalType={formData.cashflowIntervalType}
        onSelectDateIntervalType={(type) => updateFormField('cashflowIntervalType', type)}
      />
      <FrequencyField
        selectedFrequency={formData.cashflowFrequency}
        onSelectFrequency={(frequency) => updateFormField('cashflowFrequency', frequency)}
      />
    </AppSection>
  );
};
