import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ConfigService } from '@app/core/config/config.service';
import { map, Observable } from 'rxjs';
import { Category, CategoryAdapter } from '../../models/category.model';

const apiRoute: string = 'budgeting/api/v2/categories';

@Injectable({
  providedIn: 'root',
})
export class CategoriesApiService {
  constructor(private http: HttpClient, private configService: ConfigService, private adapter: CategoryAdapter) { }

  find(): Observable<Category[]> {
    return this.http.get(`${this.configService.config.apiUrl}/${apiRoute}`)
      .pipe(
        map((data: any[]) => data.map(this.adapter.adapt))
      )
  }

  findOneById(id: number | string): Observable<Category> {
    return this.http.get(`${this.configService.config.apiUrl}/${apiRoute}/${id}`)
      .pipe(map(this.adapter.adapt));
  }
}
