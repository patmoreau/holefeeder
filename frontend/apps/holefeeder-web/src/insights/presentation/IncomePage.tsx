import { tk } from '@holefeeder/shared/core';
import { useIncome } from '../api/use-income';
import { BreakdownPage, type BreakdownKind } from './BreakdownPage';

const income: BreakdownKind = {
  testID: 'income',
  title: tk.insights.income.title,
  currentTitle: tk.insights.income.currentTitle,
  categoriesTitle: tk.insights.income.categoryBreakdown.title,
  categoriesEmpty: tk.insights.income.categoryBreakdown.empty,
  tagsTitle: tk.insights.income.tagBreakdown.title,
  tagsEmpty: tk.insights.income.tagBreakdown.empty,
  higherIsGood: true,
  combinable: false,
  useBreakdown: useIncome,
};

export const IncomePage = () => <BreakdownPage kind={income} />;
