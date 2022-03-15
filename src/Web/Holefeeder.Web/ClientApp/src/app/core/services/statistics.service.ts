import {Injectable} from '@angular/core';
import {ICategoryInfo} from '@app/shared/interfaces/category-info.interface';
import {Settings} from '@app/core/models/settings.model';
import {Observable} from 'rxjs';
import {StatisticsApiService} from './api/statistics-api.service';
import {Statistics} from '../models/statistics.model';

@Injectable({providedIn: 'root'})
export class StatisticsService {

  constructor(private apiService: StatisticsApiService) {
  }

  find(settings: Settings): Observable<Statistics<ICategoryInfo>[]> {
    return this.apiService.find(settings);
  }

  findByCategoryId(id: string, settings: Settings): Observable<Statistics<ICategoryInfo>[]> {
    return this.apiService.findByCategoryId(id, settings);
  }
}
