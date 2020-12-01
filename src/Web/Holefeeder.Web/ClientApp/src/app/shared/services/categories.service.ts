import { Injectable } from '@angular/core';

import { ApiService } from '@app/shared/services/api.service';
import { ICategory } from '../interfaces/category.interface';
import { HttpParams } from '@angular/common/http';

@Injectable()
export class CategoriesService {
  private basePath = 'api/v1/categories';

  constructor(private api: ApiService) {}

  find(offset: number, limit: number, sort: string[]): Promise<ICategory[]> {
    let params = new HttpParams();
    if (offset) {
      params = params.set('offset', `${offset}`);
    }
    if (limit) {
      params = params.set('limit', `${limit}`);
    }
    if (sort) {
      sort.forEach(element => {
        params = params.append('sort', `${element}`);
      });
    }
    return this.api.get(this.basePath, params).toPromise();
  }

  findOneById(id: number | string): Promise<ICategory> {
    return this.api.get(`${this.basePath}/${id}`).toPromise();
  }
}
