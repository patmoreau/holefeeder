import { useApiCommand } from '@/shared/api/use-api-command';
import type { DashboardUpcomingFlow } from '../domain/dashboard-upcoming-flow';

export type PayChanges = {
  date: string;
  amount: number;
  updateRecurringAmount: boolean;
};

export const useUpcomingActions = () => {
  const command = useApiCommand();

  const pay = (flow: DashboardUpcomingFlow, changes?: PayChanges) =>
    command('/api/v2/cashflows/pay-cashflow', {
      cashflowId: flow.cashflowId,
      date: changes?.date ?? flow.date,
      amount: (changes?.amount ?? flow.amount).toFixed(2),
      cashflowDate: flow.date,
      updateRecurringAmount: String(changes?.updateRecurringAmount ?? false),
    });

  const clear = (flow: DashboardUpcomingFlow) =>
    command('/api/v2/cashflows/clear-cashflow', { cashflowId: flow.cashflowId, cashflowDate: flow.date });

  const cancel = (flow: DashboardUpcomingFlow) => command('/api/v2/cashflows/cancel-cashflow', { cashflowId: flow.cashflowId });

  return { pay: pay, clear: clear, cancel: cancel };
};
