import { Id } from '@holefeeder/shared/core';
import { useLocalSearchParams } from 'expo-router';
import { Tag } from '@/flows/core/flows/tag';
import { CashflowForm } from '@/flows/presentation/cashflows/CashflowForm';
import { CashflowFormData } from '@/flows/presentation/cashflows/core/cashflow-form-data';
import { useCashflow } from '@/flows/presentation/cashflows/core/use-cashflow';
import { CashflowFormProvider, validateCashflowForm } from '@/flows/presentation/cashflows/core/use-cashflow-form';
import { useAccounts } from '@/flows/presentation/shared/core/use-accounts';
import { useCategories } from '@/flows/presentation/shared/core/use-categories';
import { useTags } from '@/flows/presentation/shared/core/use-tags';
import { AppScreen } from '@/shared/presentation/AppScreen';
import { AppView } from '@/shared/presentation/AppView';
import { AppErrorSheet } from '@/shared/presentation/components/native/AppErrorSheet';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { useMultipleWatches, withDefault } from '@/shared/presentation/core/use-multiple-watches';
import { useStyles } from '@/shared/theme/core/use-styles';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  container: {
    ...theme.styles.containers.center,
  },
});

export const EditCashflowScreen = () => {
  const { id } = useLocalSearchParams<{ id: string }>();
  const cashflowId = Id.valid(id);
  const styles = useStyles(createStyles);

  const cashflowQuery = useCashflow(cashflowId);
  const accountsQuery = useAccounts();
  const categoriesQuery = useCategories();
  const tagsQuery = useTags();

  const { data, isLoading, errors } = useMultipleWatches({
    cashflow: () => cashflowQuery,
    accounts: withDefault(() => accountsQuery, []),
    categories: withDefault(() => categoriesQuery, []),
    tags: withDefault(() => tagsQuery, []),
  });

  if (isLoading || !data || !data.cashflow) {
    return (
      <AppView style={styles.container}>
        <AppLoadingIndicator />
        <AppErrorSheet {...errors} />
      </AppView>
    );
  }

  const { cashflow, accounts, categories, tags } = data;

  const account = accounts.find((a) => a.id === cashflow.accountId) ?? accounts[0];
  const category = categories.find((c) => c.id === cashflow.categoryId) ?? categories[0];
  const selectedTags: Tag[] = cashflow.tags.map((name) => ({ tag: name, count: 0 }));

  const initialData: CashflowFormData = {
    id: cashflow.id,
    effectiveDate: cashflow.effectiveDate,
    amount: cashflow.amount,
    description: cashflow.description,
    account,
    category,
    tags: selectedTags,
    intervalType: cashflow.intervalType,
    frequency: cashflow.frequency,
    recurrence: cashflow.recurrence,
  };

  return (
    <AppScreen>
      <CashflowFormProvider initialValue={initialData} validate={validateCashflowForm} validateOnChange>
        <CashflowForm accounts={accounts} categories={categories} tags={tags} />
      </CashflowFormProvider>
      <AppErrorSheet {...errors} />
    </AppScreen>
  );
};
