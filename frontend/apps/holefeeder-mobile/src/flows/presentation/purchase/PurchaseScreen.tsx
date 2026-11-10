import { Id, Logger } from '@holefeeder/shared/core';
import { router, useLocalSearchParams } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { PurchaseFormDefaults, PurchaseFormDefaultsErrors } from '@/flows/presentation/purchase/core/purchase-form-defaults';
import { PurchaseFormProvider, validatePurchaseForm } from '@/flows/presentation/purchase/core/use-purchase-form';
import { PurchaseForm } from '@/flows/presentation/purchase/PurchaseForm';
import { useAccounts } from '@/flows/presentation/shared/core/use-accounts';
import { useCategories } from '@/flows/presentation/shared/core/use-categories';
import { useTags } from '@/flows/presentation/shared/core/use-tags';
import { tk } from '@/i18n/translations';
import { AppScreen } from '@/shared/presentation/AppScreen';
import { AppView } from '@/shared/presentation/AppView';
import { EmptyState } from '@/shared/presentation/components/EmptyState';
import { AppErrorSheet } from '@/shared/presentation/components/native/AppErrorSheet';
import { AppList } from '@/shared/presentation/components/native/AppList';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
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
  const { accountId } = useLocalSearchParams<{ accountId?: string }>();
  const accountsQuery = useAccounts();
  const categoriesQuery = useCategories();
  const tagsQuery = useTags();
  const styles = useStyles(createStyles);
  const { t } = useTranslation();

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

  const initialData = PurchaseFormDefaults.create({
    accounts: accounts,
    categories: categories,
    accountId: accountId ? Id.valid(accountId) : undefined,
  });

  if (initialData.isFailure) {
    const missingAccount = initialData.errors.includes(PurchaseFormDefaultsErrors.missingAccount);

    return (
      <AppScreen testID="purchase-screen">
        <AppNative style={{ flex: 1 }}>
          <AppList inset>
            {missingAccount ? (
              <EmptyState
                icon={AppIconMap.account}
                title={t(tk.emptyStates.purchaseMissingAccount.title)}
                hint={t(tk.emptyStates.purchaseMissingAccount.hint)}
                action={{
                  label: t(tk.emptyStates.purchaseMissingAccount.action),
                  onPress: () => router.push('/(app)/AddAccount'),
                  testID: 'purchase-missing-account-add',
                }}
                testID="purchase-missing-account"
              />
            ) : (
              <EmptyState
                icon={AppIconMap.category}
                title={t(tk.emptyStates.purchaseMissingCategory.title)}
                hint={t(tk.emptyStates.purchaseMissingCategory.hint)}
                action={{
                  label: t(tk.emptyStates.purchaseMissingCategory.action),
                  onPress: () => router.push('/(app)/AddCategory'),
                  testID: 'purchase-missing-category-add',
                }}
                testID="purchase-missing-category"
              />
            )}
          </AppList>
        </AppNative>
      </AppScreen>
    );
  }

  return (
    <AppScreen testID="purchase-screen">
      <PurchaseFormProvider initialValue={initialData.value} validate={validatePurchaseForm} validateOnChange>
        <PurchaseForm accounts={accounts!} categories={categories!} tags={tags!} />
      </PurchaseFormProvider>
      <AppErrorSheet {...errors} />
    </AppScreen>
  );
};

export default PurchaseScreen;
