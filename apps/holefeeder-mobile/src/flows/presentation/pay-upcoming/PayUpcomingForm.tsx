import { Money } from '@holefeeder/shared/core';
import { useNavigation } from 'expo-router';
import { useHeaderHeight } from 'expo-router/react-navigation';
import { useTranslation } from 'react-i18next';
import { usePayUpcomingForm } from '@/flows/presentation/pay-upcoming/core/use-pay-upcoming-form';
import { PayUpcomingFormContent } from '@/flows/presentation/pay-upcoming/PayUpcomingFormContent';
import { tk } from '@/i18n/translations';
import { AppButton } from '@/shared/presentation/components/app/AppButton';
import { AppColumn } from '@/shared/presentation/components/app/AppColumn';
import { AppForm } from '@/shared/presentation/components/app/AppForm';
import { AppRow } from '@/shared/presentation/components/app/AppRow';
import { AppSpacer } from '@/shared/presentation/components/app/AppSpacer';
import { useFormActions } from '@/shared/presentation/core/use-form-actions';

export const PayUpcomingForm = ({ description }: { description: string }) => {
  const { t } = useTranslation();
  const { formData, saveForm, isDirty, errors } = usePayUpcomingForm();
  const { handleSave, handleCancel } = useFormActions({ saveForm, isDirty, errors });
  const navigation = useNavigation();
  const headerHeight = useHeaderHeight();

  navigation.setOptions({ title: description.length > 0 ? `${description}` : t(tk.payUpcoming.title) });

  return (
    <AppForm style={{ flex: 1, paddingTop: headerHeight }} contentContainerStyle={{ flexGrow: 1 }}>
      <AppColumn spacing={8}>
        <PayUpcomingFormContent />
        <AppRow spacing={8}>
          <AppSpacer />
          <AppButton
            variant="secondary"
            label={t(tk.payUpcoming.clear)}
            onPress={() => {
              formData.amount = Money.ZERO;
              formData.date = formData.cashflowDate;
              handleCancel();
            }}
          />
          <AppButton variant="primary" label={t(tk.payUpcoming.pay)} onPress={handleSave} />
          <AppSpacer />
        </AppRow>
      </AppColumn>
    </AppForm>
  );
};
