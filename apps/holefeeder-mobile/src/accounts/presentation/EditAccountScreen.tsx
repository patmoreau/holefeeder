import { DateOnly, Id } from '@holefeeder/core';
import { useLocalSearchParams } from 'expo-router';
import React from 'react';
import { EditAccountFormData } from '@/accounts/presentation/core/edit-account-form-data';
import { useAccount } from '@/accounts/presentation/core/use-account';
import { EditAccountFormProvider, validateEditAccountForm } from '@/accounts/presentation/core/use-edit-account-form';
import { EditAccountForm } from '@/accounts/presentation/EditAccountForm';
import { AppScreen } from '@/shared/presentation/AppScreen';
import { AppView } from '@/shared/presentation/AppView';
import { LoadingIndicator } from '@/shared/presentation/components/LoadingIndicator';
import { useStyles } from '@/shared/theme/core/use-styles';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  container: {
    ...theme.styles.containers.center,
  },
});

export const EditAccountScreen = () => {
  const { id } = useLocalSearchParams<{ id: string }>();
  const accountId = Id.valid(id);
  const styles = useStyles(createStyles);

  const accountResult = useAccount(accountId);

  if (!accountResult.isSuccess) {
    return (
      <AppView style={styles.container}>
        <LoadingIndicator />
      </AppView>
    );
  }

  const account = accountResult.value;

  const initialData: EditAccountFormData = {
    id: account.id,
    name: account.name,
    type: account.type,
    openBalance: account.openBalance,
    openDate: DateOnly.valid(account.openDate),
    description: account.description,
    favorite: account.favorite,
    inactive: account.inactive,
  };

  return (
    <AppScreen>
      <EditAccountFormProvider initialValue={initialData} validate={validateEditAccountForm} validateOnChange>
        <EditAccountForm />
      </EditAccountFormProvider>
    </AppScreen>
  );
};
