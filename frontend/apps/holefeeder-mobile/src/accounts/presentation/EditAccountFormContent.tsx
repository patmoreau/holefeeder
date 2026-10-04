import React from 'react';
import { useTranslation } from 'react-i18next';
import { AccountTypes } from '@/accounts/core/account-type';
import { useEditAccountForm } from '@/accounts/presentation/core/use-edit-account-form';
import { tk } from '@/i18n/translations';
import { AmountHeroForm } from '@/shared/presentation/components/fields/AmountHeroForm';
import { DateField } from '@/shared/presentation/components/fields/DateField';
import { DescriptionField } from '@/shared/presentation/components/fields/DescriptionField';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppPicker } from '@/shared/presentation/components/native/AppPicker';
import { AppSwitch } from '@/shared/presentation/components/native/AppSwitch';
import { AppTextInput } from '@/shared/presentation/components/native/AppTextInput';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

const accountTypeOptions = Object.values(AccountTypes).map((type) => ({
  id: type,
  label: type,
}));

// See BudgetSettingsFormContent for why onboarding puts its action in a footer row.
export const EditAccountFormContent = ({ footer }: { footer?: React.ReactNode }) => {
  const { t } = useTranslation();
  const { formData, updateFormField, errors } = useEditAccountForm();

  const selectedTypeOption = accountTypeOptions.find((o) => o.id === formData.type) ?? accountTypeOptions[0];

  return (
    <AmountHeroForm
      caption={t(tk.accountEdit.openBalance)}
      amount={formData.openBalance}
      onAmountChange={(value) => updateFormField('openBalance', value)}
    >
      <AppFieldSection>
        <AppField icon={AppIconMap.name} label={t(tk.accountEdit.name)} error={errors.name ? t(tk.accountEdit.errors.nameRequired) : undefined}>
          <AppTextInput
            placeholder={t(tk.accountEdit.name)}
            value={formData.name}
            onChangeText={(value) => updateFormField('name', value)}
            testID="account-name-input"
          />
        </AppField>
        <AppField icon={AppIconMap.type} label={t(tk.accountEdit.type)}>
          <AppPicker
            options={accountTypeOptions}
            selectedOption={selectedTypeOption}
            onSelectOption={(option) => updateFormField('type', option.id)}
            onOptionLabel={(option) => t(tk.accountEdit.accountTypes[option.id])}
          />
        </AppField>
      </AppFieldSection>
      <AppFieldSection>
        <DateField
          label={t(tk.accountEdit.openDate)}
          selectedDate={formData.openDate}
          onDateSelected={(date) => updateFormField('openDate', date)}
        />
      </AppFieldSection>
      <AppFieldSection>
        <DescriptionField description={formData.description} onDescriptionChange={(value) => updateFormField('description', value)} />
      </AppFieldSection>
      <AppFieldSection>
        <AppField icon={AppIconMap.favorite} label={t(tk.accountEdit.favorite)}>
          <AppSwitch value={formData.favorite} onChange={(value) => updateFormField('favorite', value)} />
        </AppField>
        {/* An account that does not exist yet cannot be inactive, and offering the
            choice during onboarding would let someone start with an account the app
            then hides from them. Deactivating is for accounts that already exist. */}
        {formData.id !== null && (
          <AppField icon={AppIconMap.inactive} label={t(tk.accountEdit.inactive)}>
            <AppSwitch value={formData.inactive} onChange={(value) => updateFormField('inactive', value)} />
          </AppField>
        )}
      </AppFieldSection>
      {footer}
    </AmountHeroForm>
  );
};
