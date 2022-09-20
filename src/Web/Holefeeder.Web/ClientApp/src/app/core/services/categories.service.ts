import { Injectable } from '@angular/core';
import { Category } from '@app/shared/models';
import { filter, mergeMap, Observable } from 'rxjs';
import { CategoriesApiService } from './api/categories-api.service';
import { StateService } from './state.service';

interface CategoriesState {
  categories: Category[];
}

const initialState: CategoriesState = {
  categories: [],
};

@Injectable({ providedIn: 'root' })
export class CategoriesService extends StateService<CategoriesState> {
  categories$: Observable<Category[]> = this.select(state => state.categories);

  constructor(private apiService: CategoriesApiService) {
    super(initialState);

    this.apiService.find().subscribe(items => {
      this.setState({ categories: items });
    });
  }

  findOneById(id: string): Observable<Category> {
    return this.select(state => state.categories).pipe(
      mergeMap((data: any[]) => data),
      filter(c => c.id === id)
    );
  }

  findOneByIndex(index: number): Category {
    return this.state.categories[index];
  }
}
