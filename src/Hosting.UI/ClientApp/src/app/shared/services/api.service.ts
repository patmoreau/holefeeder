import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { environment } from '@env/environment';

@Injectable()
export class ApiService {
  constructor(private http: HttpClient) { }

  private formatErrors(error: any) {
    console.error(error);
    return throwError(error.error);
  }

  get(path: string, params: HttpParams = new HttpParams()): Observable<any> {
    return this.http.get(`${environment.api_url}${path}`, {
      observe: 'response',
      params: params
    }).pipe(
      map(resp =>
        resp.headers.has('X-Total-Count')
          ? { totalCount: +resp.headers.get('X-Total-Count'), items: resp.body }
          : resp.body
      ),
      catchError(this.formatErrors)
    );
  }

  put(path: string, body: Object = {}): Observable<any> {
    return this.http.put(`${environment.api_url}${path}`, body)
      .pipe(
        catchError(this.formatErrors)
      );
  }

  patch(path: string, body: Object = {}): Observable<any> {
    return this.http.patch(`${environment.api_url}${path}`, body)
      .pipe(
        catchError(this.formatErrors)
      );
  }

  post(path: string, body: Object = {}): Observable<any> {
    return this.http.post(`${environment.api_url}${path}`, body)
      .pipe(
        catchError(this.formatErrors)
      );
  }

  delete(path: string): Observable<any> {
    return this.http.delete(`${environment.api_url}${path}`)
      .pipe(
        catchError(this.formatErrors)
      );
  }
}
