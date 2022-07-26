
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
