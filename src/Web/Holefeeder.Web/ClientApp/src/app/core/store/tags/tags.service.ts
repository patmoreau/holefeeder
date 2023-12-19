import { Inject, Injectable } from '@angular/core';
import { Category, CategoryType } from '@app/shared/models';
import { catchError, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { formatErrors } from '@app/core/utils/api.utils';
import { map } from 'rxjs/operators';
import { Tag } from '@app/shared/models/tag.model';

const apiRoute = 'api/v2/tags';

@Injectable({ providedIn: 'root' })
export class TagsService {
  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private apiUrl: string
  ) {}
  public fetch(): Observable<ReadonlyArray<Tag>> {
    return this.http
      .get<ReadonlyArray<Tag>>(`${this.apiUrl}/${apiRoute}`)
      .pipe(catchError(formatErrors));
  }
}