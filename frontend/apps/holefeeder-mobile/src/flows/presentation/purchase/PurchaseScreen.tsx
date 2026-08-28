import { Logger } from '@holefeeder/shared/core';
import { PurchaseFormDefaults } from '@/flows/presentation/purchase/core/purchase-form-defaults';
import { PurchaseFormProvider, validatePurchaseForm } from '@/flows/presentation/purchase/core/use-purchase-form';
import { PurchaseForm } from '@/flows/presentation/purchase/PurchaseForm';
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

const logger = Logger.create('PurchaseScreen');

const createStyles = (theme: Theme) => ({
  container: {
    ...theme.styles.containers.center,
  },
});

const PurchaseScreen = () => {
  logger.debug('Rendering PurchaseScreen');
  const accountsQuery = useAccounts();
  const categoriesQuery = useCategories();
  const tagsQuery = useTags();
  const styles = useStyles(createStyles);

  const { data, isLoading, errors } = useMultipleWatches({
    accounts: withDefault(() => accountsQuery, []),
    categories: withDefault(() => categoriesQuery, []),
    tags: withDefault(() => tagsQuery, []),
  });

  if (isLoading || !data) {
    return (
      <AppView style={styles.container}>
        <AppLoadingIndicator />
        <AppErrorSheet {...errors} />
      </AppView>
    );
  }

  const { accounts, categories, tags } = data;

  const initialData = PurchaseFormDefaults.create({ accounts: accounts, categories: categories });

  return (
    <AppScreen testID="purchase-screen">
      <PurchaseFormProvider initialValue={initialData} validate={validatePurchaseForm} validateOnChange>
        <PurchaseForm accounts={accounts!} categories={categories!} tags={tags!} />
      </PurchaseFormProvider>
      <AppErrorSheet {...errors} />
    </AppScreen>
  );
};

export default PurchaseScreen;
