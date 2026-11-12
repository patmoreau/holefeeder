import { useApiCommand } from '@/shared/api/use-api-command';

export type CashflowChanges = { amount: number; effectiveDate: string; description: string; tags: string[] };

export const useModifyCashflow = () => {
  const command = useApiCommand();

  return (id: string, changes: CashflowChanges) =>
    command('/api/v2/cashflows/modify', {
      id: id,
      amount: changes.amount.toFixed(2),
      effectiveDate: changes.effectiveDate,
      description: changes.description,
      tags: changes.tags,
    });
};
