import { listStyle } from '@expo/ui/swift-ui/modifiers';
import { AppView } from '@/shared/presentation/AppView';
import { AppList } from '@/shared/presentation/components/native/AppList';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { CategorySpendingList } from './CategorySpendingList';
import { InsightsPeriodHeader } from './InsightsPeriodHeader';
import { TagSpendingList } from './TagSpendingList';

export default function InsightsScreen() {
  return (
    <AppView style={{ flex: 1 }}>
      <InsightsPeriodHeader />
      <AppNative style={{ flex: 1 }}>
        <AppList modifiers={[listStyle('inset')]}>
          <CategorySpendingList />
          <TagSpendingList />
        </AppList>
      </AppNative>
    </AppView>
  );
}
