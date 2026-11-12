export type CategoryType = 'expense' | 'gain';

export type Category = {
  id: string;
  name: string;
  type: CategoryType;
  color: string;
  budgetAmount: number;
  favorite: boolean;
  system: boolean;
};

const valid = (value: Record<string, unknown>): Category => ({
  id: value.id as string,
  name: value.name as string,
  type: value.type as CategoryType,
  color: value.color as string,
  budgetAmount: value.budgetAmount as number,
  favorite: value.favorite as boolean,
  system: value.system as boolean,
});

export const Category = {
  valid: valid,
};
