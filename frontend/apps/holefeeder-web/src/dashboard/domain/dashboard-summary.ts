export type DashboardSummary = {
  currentExpenses: number;
  expenseVariation: number;
  expenseVariationPercentage: number;
  netFlow: number;
  currentGains: number;
  averageExpenses: number;
  projectedNetFlow: number;
};

const valid = (value: Record<string, unknown>): DashboardSummary => ({
  currentExpenses: value.currentExpenses as number,
  expenseVariation: value.expenseVariation as number,
  expenseVariationPercentage: value.expenseVariationPercentage as number,
  netFlow: value.netFlow as number,
  currentGains: value.currentGains as number,
  averageExpenses: value.averageExpenses as number,
  projectedNetFlow: value.projectedNetFlow as number,
});

export const DashboardSummary = { valid: valid };
