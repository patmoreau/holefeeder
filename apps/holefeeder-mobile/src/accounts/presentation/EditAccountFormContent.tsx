import React from 'react';
import { useTranslation } from 'react-i18next';
import { AccountTypes } from '@/accounts/core/account-type';
import { useEditAccountForm } from '@/accounts/presentation/core/use-edit-account-form';
import { AmountField } from '@/flows/presentation/shared/components/AmountField';
import { DescriptionField } from '@/flows/presentation/shared/components/DescriptionField';
import { tk } from '@/i18n/translations';
import { AppIconMap } from '@/shared/presentation/components/app/app-icon-map';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppFieldSection } from '@/shared/presentation/components/app/AppFieldSection';
import { AppForm } from '@/shared/presentation/components/app/AppForm';
import { AppPicker } from '@/shared/presentation/components/app/AppPicker';
import { AppSwitch } from '@/shared/presentation/components/app/AppSwitch';
import { AppTextInput } from '@/shared/presentation/components/app/AppTextInput';
import { DateField } from '@/shared/presentation/fields/DateField';

const accountTypeOptions = Object.values(AccountTypes).map((type) => ({ id: type, label: type }));

export const EditAccountFormContent = () => {
  const { t } = useTranslation();
  const { formData, updateFormField, errors } = useEditAccountForm();

  const selectedTypeOption = accountTypeOptions.find((o) => o.id === formData.type) ?? accountTypeOptions[0];

  return (
    <AppForm>
      <AppFieldSection>
        <AppField
          icon={AppIconMap.account}
          label={t(tk.accountEdit.name)}
          error={errors.name ? t(tk.accountEdit.errors.nameRequired) : undefined}
        >
          <AppTextInput placeholder={t(tk.accountEdit.name)} value={formData.name} onChangeText={(value) => updateFormField('name', value)} />
        </AppField>
        <AppField icon={AppIconMap.category} label={t(tk.accountEdit.type)}>
          <AppPicker
            options={accountTypeOptions}
            selectedOption={selectedTypeOption}
            onSelectOption={(option) => updateFormField('type', option.id)}
            onOptionLabel={(option) => t(tk.accountEdit.accountTypes[option.id])}
          />
        </AppField>
      </AppFieldSection>
      <AppFieldSection>
        <AmountField amount={formData.openBalance} onAmountChange={(value) => updateFormField('openBalance', value)} />
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
        <AppField icon={AppIconMap.add} label={t(tk.accountEdit.favorite)}>
          <AppSwitch value={formData.favorite} onChange={(value) => updateFormField('favorite', value)} />
        </AppField>
        <AppField icon={AppIconMap.close} label={t(tk.accountEdit.inactive)}>
          <AppSwitch value={formData.inactive} onChange={(value) => updateFormField('inactive', value)} />
        </AppField>
      </AppFieldSection>
    </AppForm>
  );
};
