import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { useSettingsForm } from '@/settings/presentation/core/use-settings-form';
import { DateField } from '@/shared/presentation/components/fields/DateField';
import { DateIntervalTypeField } from '@/shared/presentation/components/fields/DateIntervalTypeField';
import { FrequencyField } from '@/shared/presentation/components/fields/FrequencyField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppForm } from '@/shared/presentation/components/native/AppForm';

// footer lets a caller add a row of its own — onboarding puts its Continue button
// there. Inside the form, because a button placed after one is clipped past the home
// indicator, and the header toolbar takes no testID for the flows to select.
export const BudgetSettingsFormContent = ({ footer }: { footer?: React.ReactNode }) => {
  const { t } = useTranslation();
  const { formData, updateFormField } = useSettingsForm();

  return (
    <AppForm>
      <AppFieldSection title={t(tk.budgetSettings.section)}>
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
      </AppFieldSection>
      {footer}
    </AppForm>
  );
};
