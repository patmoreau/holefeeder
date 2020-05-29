export enum CategoryType {
    expense = 'Expense',
    gain = 'Gain',
}

export function categoryTypeMultiplier(type: CategoryType): number {
    return type === CategoryType.expense ? -1 : 1;
}
