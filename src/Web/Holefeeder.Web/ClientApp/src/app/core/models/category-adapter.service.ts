import { Injectable } from '@angular/core';
import { Category } from '@app/core/models/category.model';
import { Adapter } from '@app/shared/models';

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
