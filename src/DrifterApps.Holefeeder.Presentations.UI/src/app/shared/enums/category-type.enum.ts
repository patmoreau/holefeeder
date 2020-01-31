export enum CategoryType {
    expense = 'expense',
    gain = 'gain',
}

export function categoryTypeMultiplier(type: CategoryType): number {
    return type === CategoryType.expense ? -1 : 1;
}
