import {HttpClient, HttpParams} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable, throwError} from 'rxjs';
import {catchError, map} from 'rxjs/operators';
import {AppConfigService} from "@app/app-config.service";

@Injectable()
export class ApiService {

  constructor(private http: HttpClient, private configService: AppConfigService) {
  }

  private formatErrors(error: any) {
    console.error(error);
    return throwError(error.error);
  }

  get(path: string, params: HttpParams = new HttpParams()): Observable<any> {
    return this.http.get(`${this.configService.getConfig().ApiUrl}${path}`, {
      observe: 'response',
      params: params
    }).pipe(
      map(resp =>
        resp.headers.has('X-Total-Count')
          ? {totalCount: +resp.headers.get('X-Total-Count'), items: resp.body}
          : resp.body
      ),
      catchError(this.formatErrors)
    );
  }

  put(path: string, body: Object = {}): Observable<any> {
    return this.http.put(`${this.configService.getConfig().ApiUrl}${path}`, body)
      .pipe(
        catchError(this.formatErrors)
      );
  }

  patch(path: string, body: Object = {}): Observable<any> {
    return this.http.patch(`${this.configService.getConfig().ApiUrl}${path}`, body)
      .pipe(
        catchError(this.formatErrors)
      );
  }

  post(path: string, body: Object = {}): Observable<any> {
    return this.http.post(`${this.configService.getConfig().ApiUrl}${path}`, body)
      .pipe(
        catchError(this.formatErrors)
      );
  }

  delete(path: string): Observable<any> {
    return this.http.delete(`${this.configService.getConfig().ApiUrl}${path}`)
      .pipe(
        catchError(this.formatErrors)
      );
  }
}
