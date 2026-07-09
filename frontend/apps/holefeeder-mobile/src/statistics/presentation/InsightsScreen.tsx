import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppScreen } from '@/shared/presentation/AppScreen';
import { AppForm } from '@/shared/presentation/components/native/AppForm';
import { ScreenTitle } from '@/shared/presentation/ScreenTitle';
import { CategorySpendingList } from './CategorySpendingList';
import { InsightsPeriodHeader } from './InsightsPeriodHeader';
import { TagSpendingList } from './TagSpendingList';

export default function InsightsScreen() {
  const { t } = useTranslation();

  return (
    <AppScreen>
      <ScreenTitle title={t(tk.insights.title)} />
      <InsightsPeriodHeader />
      <AppForm>
        <CategorySpendingList />
        <TagSpendingList />
      </AppForm>
    </AppScreen>
  );
}
