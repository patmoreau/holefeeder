export function categoryTypeMultiplier(type: string): number {
  return type === 'Expense' ? -1 : 1;
}
