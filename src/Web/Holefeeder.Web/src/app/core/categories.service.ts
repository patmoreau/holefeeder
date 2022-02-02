import { Injectable } from '@angular/core';
import { StateService } from '@app/core/state.service';
import { filter, mergeMap, Observable } from 'rxjs';
import { Category } from './models/category.model';
import { CategoriesApiService } from './api/categories-api.service';

interface CategoriesState {
  categories: Category[];
}

const initialState: CategoriesState = {
  categories: undefined,
};

@Injectable({
  providedIn: 'root',
})
export class CategoriesService extends StateService<CategoriesState> {

  categories$: Observable<Category[]> = this.select((state) => state.categories);

  constructor(private apiService: CategoriesApiService) {
    super(initialState);

    this.apiService.find()
      .subscribe(items => {
        this.setState({ categories: items });
      });
  }

  findOneById(id: string): Observable<Category> {
    return this.select((state) => state.categories)
      .pipe(
        mergeMap((data: any[]) => data),
        filter(c => c.id === id)
      );
  }

  findOneByIndex(index: number): Category {
    return this.state.categories[index];
  }
}
