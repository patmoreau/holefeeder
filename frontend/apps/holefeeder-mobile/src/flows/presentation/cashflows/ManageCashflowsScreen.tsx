import { useTranslation } from 'react-i18next';
import { Cashflow } from '@/flows/core/flows/cashflow';
import { CashflowCard } from '@/flows/presentation/cashflows/components/CashflowCard';
import { useCashflows } from '@/flows/presentation/cashflows/core/use-cashflows';
import { useCategories } from '@/flows/presentation/shared/core/use-categories';
import { tk } from '@/i18n/translations';
import { AppScreen } from '@/shared/presentation/AppScreen';
import { AppView } from '@/shared/presentation/AppView';
import { EmptyState } from '@/shared/presentation/components/EmptyState';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppForm } from '@/shared/presentation/components/native/AppForm';
import { AppListForEach } from '@/shared/presentation/components/native/AppListForEach';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useStyles } from '@/shared/theme/core/use-styles';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  container: {
    ...theme.styles.containers.center,
  },
});

export const ManageCashflowsScreen = () => {
  const { t } = useTranslation();
  const styles = useStyles(createStyles);
  const cashflowsResult = useCashflows();
  const categoriesResult = useCategories();

  const category = (cashflow: Cashflow) =>
    categoriesResult.isSuccess ? categoriesResult.value.find((c) => c.id === cashflow.categoryId) : undefined;

  if (!cashflowsResult.isSuccess) {
    return (
      <AppView style={styles.container}>
        <AppLoadingIndicator />
      </AppView>
    );
  }

  return (
    <AppScreen>
      <AppForm>
        {cashflowsResult.value.length === 0 && (
          <EmptyState
            icon={AppIconMap.cashflow}
            title={t(tk.emptyStates.cashflows.title)}
            hint={t(tk.emptyStates.cashflows.hint)}
            testID="manage-cashflows-empty"
          />
        )}
        {cashflowsResult.value.length > 0 && (
          <AppFieldSection title={t(tk.manageCashflows.title)}>
            <AppListForEach>
              {cashflowsResult.value.map((cashflow) => (
                <CashflowCard
                  key={cashflow.id}
                  cashflow={cashflow}
                  categoryName={category(cashflow)?.name ?? ''}
                  color={category(cashflow)?.color}
                />
              ))}
            </AppListForEach>
          </AppFieldSection>
        )}
      </AppForm>
    </AppScreen>
  );
};
