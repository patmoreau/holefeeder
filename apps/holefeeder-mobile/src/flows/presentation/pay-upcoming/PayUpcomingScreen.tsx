import { useLocalSearchParams } from 'expo-router';
import React from 'react';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { PayUpcomingFormData } from '@/flows/presentation/pay-upcoming/core/pay-upcoming-form-data';
import { PayUpcomingFormProvider, validatePayUpcomingForm } from '@/flows/presentation/pay-upcoming/core/use-pay-upcoming-form';
import { PayUpcomingForm } from '@/flows/presentation/pay-upcoming/PayUpcomingForm';
import { AppModal } from '@/shared/presentation/AppModal';
import { LoadingIndicator } from '@/shared/presentation/components/LoadingIndicator';

const PayUpcomingScreen = () => {
  const { data: upcomingFlowParam } = useLocalSearchParams<{ data: string }>();
  const upcomingFlowResult = UpcomingFlow.create(JSON.parse(upcomingFlowParam));

  if (upcomingFlowResult.isFailure) {
    return (
      <AppModal>
        <LoadingIndicator />
      </AppModal>
    );
  }

  const initialData: PayUpcomingFormData = {
    cashflowId: upcomingFlowResult.value.id,
    cashflowDate: upcomingFlowResult.value.date,
    date: upcomingFlowResult.value.date,
    amount: upcomingFlowResult.value.amount,
  };

  return (
    <AppModal>
      <PayUpcomingFormProvider initialValue={initialData} validate={validatePayUpcomingForm} validateOnChange>
        <PayUpcomingForm description={upcomingFlowResult.value.description} />
      </PayUpcomingFormProvider>
    </AppModal>
  );
};

export default PayUpcomingScreen;
