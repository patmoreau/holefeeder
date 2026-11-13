import { type CategoryType } from '@holefeeder/shared/core';
import { useApiCommand } from '@/shared/api/use-api-command';

export type CategoryChanges = { name: string; type: CategoryType; color: string; budgetAmount: number; favorite: boolean };

const bodyOf = (changes: CategoryChanges) => ({
  name: changes.name,
  type: changes.type,
  color: changes.color,
  budgetAmount: changes.budgetAmount.toFixed(2),
  favorite: changes.favorite,
});

export const useSaveCategory = () => {
  const command = useApiCommand();

  return (id: string | null, changes: CategoryChanges) =>
    id === null ? command('/api/v2/categories', bodyOf(changes)) : command('/api/v2/categories/modify', { id: id, ...bodyOf(changes) });
};
