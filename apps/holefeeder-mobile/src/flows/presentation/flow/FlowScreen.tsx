import { useLocalSearchParams } from 'expo-router';
import React from 'react';
import { FlowFormData } from '@/flows/presentation/flow/core/flow-form-data';
import { useFlow } from '@/flows/presentation/flow/core/use-flow';
import { FlowFormProvider, validateFormForm } from '@/flows/presentation/flow/core/use-flow-form';
import { FlowForm } from '@/flows/presentation/flow/FlowForm';
import { useAccounts } from '@/flows/presentation/shared/core/use-accounts';
import { useCategories } from '@/flows/presentation/shared/core/use-categories';
import { useTags } from '@/flows/presentation/shared/core/use-tags';
import { Id } from '@/shared/core/id';
import { AppScreen } from '@/shared/presentation/AppScreen';
import { AppView } from '@/shared/presentation/AppView';
import { ErrorSheet } from '@/shared/presentation/components/ErrorSheet';
import { LoadingIndicator } from '@/shared/presentation/components/LoadingIndicator';
import { useMultipleWatches, withDefault } from '@/shared/presentation/core/use-multiple-watches';
import { useStyles } from '@/shared/theme/core/use-styles';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  container: {
    ...theme.styles.containers.center,
  },
});

const FlowScreen = () => {
  const { id } = useLocalSearchParams<{ id: string }>();
  const flowId = Id.valid(id);

  const accountsQuery = useAccounts();
  const categoriesQuery = useCategories();
  const tagsQuery = useTags();
  const flowQuery = useFlow(flowId);
  const styles = useStyles(createStyles);

  const { data, isLoading, errors } = useMultipleWatches({
    accounts: withDefault(() => accountsQuery, []),
    categories: withDefault(() => categoriesQuery, []),
    tags: withDefault(() => tagsQuery, []),
    flow: () => flowQuery,
  });

  if (isLoading || !data) {
    return (
      <AppView style={styles.container}>
        <LoadingIndicator />
        <ErrorSheet {...errors} />
      </AppView>
    );
  }

  const { accounts, categories, tags, flow } = data;

  if (!flow) {
    return (
      <AppView style={styles.container}>
        <ErrorSheet {...errors} />
      </AppView>
    );
  }

  const initialData: FlowFormData = {
    id: flow.id,
    flowType: flow.categoryType,
    date: flow.date,
    amount: flow.amount,
    description: flow.description,
    account: accounts.find((account) => account.id === flow.accountId)!,
    category: categories.find((category) => category.id === flow.categoryId)!,
    tags: flow.tags.map((tag) => tags.find((t) => t.tag === tag)!),
  };

  return (
    <AppScreen>
      <FlowFormProvider initialValue={initialData} validate={validateFormForm} validateOnChange>
        <FlowForm accounts={accounts!} categories={categories!} tags={tags!} />
      </FlowFormProvider>
      <ErrorSheet {...errors} />
    </AppScreen>
  );
};

export default FlowScreen;
