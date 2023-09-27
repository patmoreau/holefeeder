import { Inject, Injectable } from '@angular/core';
import { Category } from '@app/shared/models';
import { catchError, Observable, retry } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { formatErrors } from '@app/core/utils/api.utils';
import { map } from 'rxjs/operators';

const apiRoute = 'api/v2/categories';

@Injectable({ providedIn: 'root' })
export class CategoriesService {
  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private apiUrl: string
  ) {}
  public fetch(): Observable<ReadonlyArray<Category>> {
    return this.http.get(`${this.apiUrl}/${apiRoute}`).pipe(
      map((data: any) =>
        data.map(
          (item: any) =>
            new Category(
              item.id,
              item.name,
              item.description,
              item.color,
              item.icon,
              item.inactive
            )
        )
      ),
      catchError(formatErrors)
    );
  }
}
