import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { useSettingsForm } from '@/settings/presentation/core/use-settings-form';
import { AppForm } from '@/shared/presentation/AppForm';
import { AppSection } from '@/shared/presentation/AppSection';
import { DateField } from '@/shared/presentation/fields/DateField';
import { DateIntervalTypeField } from '@/shared/presentation/fields/DateIntervalTypeField';
import { FrequencyField } from '@/shared/presentation/fields/FrequencyField';

export const BudgetSettingsFormContent = () => {
  const { t } = useTranslation();
  const { formData, updateFormField } = useSettingsForm();

  return (
    <AppForm>
      <AppSection title={t(tk.budgetSettings.section)}>
        <DateField
          label={t(tk.budgetSettings.date)}
          selectedDate={formData.effectiveDate}
          onDateSelected={(date) => updateFormField('effectiveDate', date)}
        />
        <DateIntervalTypeField
          selectedDateIntervalType={formData.intervalType}
          onSelectDateIntervalType={(type) => updateFormField('intervalType', type)}
        />
        <FrequencyField selectedFrequency={formData.frequency} onSelectFrequency={(frequency) => updateFormField('frequency', frequency)} />
      </AppSection>
    </AppForm>
  );
};
