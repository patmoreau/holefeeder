import { Injectable } from '@angular/core';
import { ICategoryInfo } from '@app/shared/models';
import { Observable } from 'rxjs';
import { Settings, Statistics } from '../models';
import { StatisticsApiService } from './api/statistics-api.service';

@Injectable({ providedIn: 'root' })
export class StatisticsService {
  constructor(private apiService: StatisticsApiService) {}

  find(settings: Settings): Observable<Statistics<ICategoryInfo>[]> {
    return this.apiService.find(settings);
  }

  findByCategoryId(
    id: string,
    settings: Settings
  ): Observable<Statistics<ICategoryInfo>[]> {
    return this.apiService.findByCategoryId(id, settings);
  }
}
