import { Injectable } from "@angular/core";
import { Adapter } from "@app/shared/interfaces/adapter.interface";

export class Category {
  constructor(
    public id: string,
    public name: string,
    public type: string,
    public color: string,
    public budgetAmount: number,
    public favorite: boolean
  ) { }
}

@Injectable({
  providedIn: "root",
})
export class CategoryAdapter implements Adapter<Category> {
  adapt(item: any): Category {
    return new Category(item.id, item.name, item.type, item.color, item.budgetAmount, item.favorite);
  }
}
