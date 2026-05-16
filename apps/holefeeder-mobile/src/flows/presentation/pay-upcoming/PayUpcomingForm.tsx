import { useHeaderHeight } from '@react-navigation/elements';
import { useNavigation } from 'expo-router';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { usePayUpcomingForm } from '@/flows/presentation/pay-upcoming/core/use-pay-upcoming-form';
import { PayUpcomingFormContent } from '@/flows/presentation/pay-upcoming/PayUpcomingFormContent';
import { tk } from '@/i18n/translations';
import { Money } from '@/shared/core/money';
import { AppForm } from '@/shared/presentation/AppForm';
import { AppButton } from '@/shared/presentation/components/AppButton';
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
      <View style={{ flexGrow: 1, flexDirection: 'column', gap: 8 }}>
        <PayUpcomingFormContent />
        <View style={{ flexDirection: 'row', justifyContent: 'center', gap: 8 }}>
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
        </View>
      </View>
    </AppForm>
  );
};
