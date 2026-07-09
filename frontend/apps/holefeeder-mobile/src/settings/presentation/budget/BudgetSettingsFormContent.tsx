import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { useSettingsForm } from '@/settings/presentation/core/use-settings-form';
import { DateField } from '@/shared/presentation/components/fields/DateField';
import { DateIntervalTypeField } from '@/shared/presentation/components/fields/DateIntervalTypeField';
import { FrequencyField } from '@/shared/presentation/components/fields/FrequencyField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppForm } from '@/shared/presentation/components/native/AppForm';

export const BudgetSettingsFormContent = () => {
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
    </AppForm>
  );
};
