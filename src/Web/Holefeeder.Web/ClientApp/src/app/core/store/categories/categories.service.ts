import { Inject, Injectable } from '@angular/core';
import { Category, CategoryType } from '@app/shared/models';
import { catchError, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { formatErrors } from '@app/core/utils/api.utils';
import { map } from 'rxjs/operators';

const apiRoute = 'api/v2/categories';

type categoryType = {
  id: string;
  name: string;
  type: CategoryType;
  color: string;
  budgetAmount: number;
  inactive: boolean;
};

@Injectable({ providedIn: 'root' })
export class CategoriesService {
  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private apiUrl: string
  ) {}
  public fetch(): Observable<ReadonlyArray<Category>> {
    return this.http
      .get<ReadonlyArray<categoryType>>(`${this.apiUrl}/${apiRoute}`)
      .pipe(
        map(data =>
          data.map(
            item =>
              new Category(
                item.id,
                item.name,
                item.type,
                item.color,
                item.budgetAmount,
                item.inactive
              )
          )
        ),
        catchError(formatErrors)
      );
  }
}
