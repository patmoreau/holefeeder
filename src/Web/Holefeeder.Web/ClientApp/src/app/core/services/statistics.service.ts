import {Injectable} from "@angular/core";
import {StatisticsApiService} from "@app/core/services/api/statistics-api.service";
import {Settings, Statistics} from "@app/core";
import {Observable} from "rxjs";
import {ICategoryInfo} from "@app/shared";

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
