import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { CategoryAdapter } from '@app/core/adapters';
import { Category } from '@app/shared/models';
import { map, Observable } from 'rxjs';

const apiRoute = 'api/v2/categories';

@Injectable({ providedIn: 'root' })
export class CategoriesApiService {
  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private apiUrl: string,
    private adapter: CategoryAdapter
  ) {}

  find(): Observable<Category[]> {
    return this.http
      .get(`${this.apiUrl}/${apiRoute}`)
      .pipe(map((data: any) => data.map(this.adapter.adapt)));
  }

  findOneById(id: number | string): Observable<Category> {
    return this.http
      .get(`${this.apiUrl}/${apiRoute}/${id}`)
      .pipe(map(this.adapter.adapt));
  }
}
