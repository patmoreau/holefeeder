import { Injectable } from '@angular/core';

import { ApiService } from '@app/shared/services/api.service';
import { ICategory } from '../interfaces/category.interface';
import { HttpParams } from '@angular/common/http';

@Injectable()
export class CategoriesService {
  private basePath = 'budgeting/api/v2/categories';

  constructor(private api: ApiService) {}

  find(): Promise<ICategory[]> {
    return this.api.get(`${this.basePath}/get-categories`).toPromise();
  }

  findOneById(id: number | string): Promise<ICategory> {
    return this.api.get(`${this.basePath}/${id}`).toPromise();
  }
}
