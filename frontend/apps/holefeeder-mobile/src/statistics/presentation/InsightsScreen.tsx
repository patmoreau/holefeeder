import { listStyle } from '@expo/ui/swift-ui/modifiers';
import { AppView } from '@/shared/presentation/AppView';
import { AppList } from '@/shared/presentation/components/native/AppList';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { CategorySpendingList } from './CategorySpendingList';
import { CombinedInsightToggle } from './CombinedInsightToggle';
import { CombinedSpendingList } from './CombinedSpendingList';
import { useCombinedInsight } from './core/use-combined-insight';
import { InsightsPeriodHeader } from './InsightsPeriodHeader';
import { TagSpendingList } from './TagSpendingList';

export default function InsightsScreen() {
  const { combined } = useCombinedInsight();

  return (
    <AppView style={{ flex: 1 }}>
      <InsightsPeriodHeader />
      <AppNative style={{ flex: 1 }}>
        <AppList modifiers={[listStyle('inset')]}>
          <CombinedInsightToggle />
          {combined ? (
            <CombinedSpendingList />
          ) : (
            <>
              <CategorySpendingList />
              <TagSpendingList />
            </>
          )}
        </AppList>
      </AppNative>
    </AppView>
  );
}
