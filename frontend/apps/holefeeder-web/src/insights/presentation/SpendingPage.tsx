import { tk } from '@holefeeder/shared/core';
import { useSpending } from '../api/use-spending';
import { BreakdownPage, type BreakdownKind } from './BreakdownPage';

const spending: BreakdownKind = {
  testID: 'spending',
  title: tk.insights.spending.title,
  currentTitle: tk.insights.spending.currentTitle,
  categoriesTitle: tk.insights.categoryBreakdown.title,
  categoriesEmpty: tk.insights.categoryBreakdown.empty,
  tagsTitle: tk.insights.tagBreakdown.title,
  tagsEmpty: tk.insights.tagBreakdown.empty,
  higherIsGood: false,
  combinable: true,
  useBreakdown: useSpending,
};

export const SpendingPage = () => <BreakdownPage kind={spending} />;
