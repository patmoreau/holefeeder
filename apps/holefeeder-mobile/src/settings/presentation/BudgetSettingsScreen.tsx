import React from 'react';
import { DefaultSettings } from '@/settings/core/settings';
import { SettingsFormData } from '@/settings/core/settings-form-data';
import { BudgetSettingsForm } from '@/settings/presentation/BudgetSettingsForm';
import { SettingsFormProvider, validateSettingsForm } from '@/settings/presentation/core/use-settings-form';
import { AppScreen } from '@/shared/presentation/AppScreen';
import { AppView } from '@/shared/presentation/AppView';
import { ErrorSheet } from '@/shared/presentation/components/ErrorSheet';
import { LoadingIndicator } from '@/shared/presentation/components/LoadingIndicator';
import { useMultipleWatches, withDefault } from '@/shared/presentation/core/use-multiple-watches';
import { useSettings } from '@/shared/presentation/core/use-settings';
import { useStyles } from '@/shared/theme/core/use-styles';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  container: {
    ...theme.styles.containers.center,
  },
});

const BudgetSettingsScreen = () => {
  const settingsQuery = useSettings();
  const styles = useStyles(createStyles);

  const { data, isLoading, errors } = useMultipleWatches({
    settings: withDefault(() => settingsQuery, DefaultSettings),
  });

  if (isLoading || !data) {
    return (
      <AppView style={styles.container}>
        <LoadingIndicator />
        <ErrorSheet {...errors} />
      </AppView>
    );
  }

  const { settings } = data;

  const initialData: SettingsFormData = {
    effectiveDate: settings.effectiveDate,
    frequency: settings.frequency,
    intervalType: settings.intervalType,
  };

  return (
    <AppScreen>
      <SettingsFormProvider initialValue={initialData} validate={validateSettingsForm} validateOnChange>
        <BudgetSettingsForm />
      </SettingsFormProvider>
      <ErrorSheet {...errors} />
    </AppScreen>
  );
};

export default BudgetSettingsScreen;
