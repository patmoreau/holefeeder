import { useApiCommand } from '@/shared/api/use-api-command';

export const useCancelCashflow = () => {
  const command = useApiCommand();

  return (id: string) => command('/api/v2/cashflows/cancel-cashflow', { cashflowId: id });
};
