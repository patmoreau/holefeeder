import { Injectable } from '@angular/core';
import { Adapter } from '@app/shared';
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

@Injectable({ providedIn: 'root' })
export class CategoryAdapter implements Adapter<Category> {
  adapt(item: any): Category {
    return new Category(
      item.id,
      item.name,
      item.type,
      item.color,
      item.budgetAmount,
      item.favorite
    );
  }
}
