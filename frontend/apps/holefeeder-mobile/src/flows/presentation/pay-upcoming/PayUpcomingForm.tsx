import { Money } from '@holefeeder/shared/core';
import { useNavigation } from 'expo-router';
import { useHeaderHeight } from 'expo-router/react-navigation';
import { useLayoutEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { usePayUpcomingForm } from '@/flows/presentation/pay-upcoming/core/use-pay-upcoming-form';
import { PayUpcomingFormContent } from '@/flows/presentation/pay-upcoming/PayUpcomingFormContent';
import { amountToneFor } from '@/flows/presentation/shared/core/amount-tone';
import { tk } from '@/i18n/translations';
import { CategoryType } from '@/shared/core/category-type';
import { AmountHeroForm } from '@/shared/presentation/components/fields/AmountHeroForm';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppRow } from '@/shared/presentation/components/native/AppRow';
import { AppSpacer } from '@/shared/presentation/components/native/AppSpacer';
import { useFormActions } from '@/shared/presentation/core/use-form-actions';

export const PayUpcomingForm = ({ description, categoryType }: { description: string; categoryType: CategoryType }) => {
  const { t } = useTranslation();
  const { formData, updateFormField, saveForm, isDirty, errors } = usePayUpcomingForm();
  const { handleSave, handleCancel } = useFormActions({
    saveForm,
    isDirty,
    errors,
  });
  const navigation = useNavigation();
  const headerHeight = useHeaderHeight();

  useLayoutEffect(() => {
    navigation.setOptions({
      title: description.length > 0 ? `${description}` : t(tk.payUpcoming.title),
    });
  }, [navigation, description, t]);

  return (
    <AmountHeroForm
      hostStyle={{ paddingTop: headerHeight }}
      caption={t(tk.purchase.basicSection.amount)}
      amount={formData.amount}
      onAmountChange={(amount) => updateFormField('amount', amount)}
      tone={amountToneFor(categoryType)}
    >
      <AppColumn spacing={8}>
        <PayUpcomingFormContent />
        <AppRow spacing={8}>
          <AppSpacer />
          <AppButton
            variant="secondary"
            label={t(tk.common.cancel)}
            onPress={() => {
              formData.amount = Money.ZERO;
              formData.date = formData.cashflowDate;
              formData.updateRecurringAmount = false;
              handleCancel();
            }}
          />
          <AppButton variant="primary" label={t(tk.payUpcoming.pay)} onPress={handleSave} />
          <AppSpacer />
        </AppRow>
      </AppColumn>
    </AmountHeroForm>
  );
};
