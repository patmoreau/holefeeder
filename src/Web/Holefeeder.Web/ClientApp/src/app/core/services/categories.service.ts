import { Inject, Injectable } from '@angular/core';
import { Category } from '@app/shared/models';
import { catchError, filter, map, mergeMap, Observable } from 'rxjs';
import { StateService } from './state.service';
import { HttpClient } from '@angular/common/http';
import { CategoryAdapter } from '@app/core/adapters';
import { formatErrors } from '@app/core/utils/api.utils';

const apiRoute = 'api/v2/categories';

interface CategoriesState {
  categories: Category[];
}

const initialState: CategoriesState = {
  categories: [],
};

@Injectable({ providedIn: 'root' })
export class CategoriesService extends StateService<CategoriesState> {
  categories$: Observable<Category[]> = this.select(state => state.categories);

  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private apiUrl: string,
    private adapter: CategoryAdapter
  ) {
    super(initialState);

    this.load();
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

  private load() {
    this.getAll().subscribe(items => this.setState({ categories: items }));
  }

  private getAll(): Observable<Category[]> {
    return this.http.get(`${this.apiUrl}/${apiRoute}`).pipe(
      map((data: any) => data.map(this.adapter.adapt)),
      catchError(formatErrors)
    );
  }
}
