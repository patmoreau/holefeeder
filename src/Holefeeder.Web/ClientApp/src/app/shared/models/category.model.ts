import { CategoryType } from '@app/shared/models';

export class Category {
  constructor(
    public id: string,
    public name: string,
    public type: CategoryType,
    public color: string,
    public budgetAmount: number,
    public favorite: boolean
  ) {}
}
