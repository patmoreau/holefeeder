import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ConfigService } from '@app/config/config.service';
import { PagingInfo } from '../interfaces/paging-info.interface';

@Injectable()
export abstract class BaseApiService<T> {
  protected abstract apiRoute: string;

  constructor(private http: HttpClient, private configService: ConfigService) { }

  get(path: string, params: HttpParams = new HttpParams()): Observable<T> {
    return this.http.get<T>(`${this.configService.config.apiUrl}/${this.apiRoute}/${path}`, {
      params: params
    }).pipe(catchError(this.formatErrors));
  }

  getAll(path: string, params: HttpParams = new HttpParams()): Observable<T[]> {
    return this.http.get<T[]>(`${this.configService.config.apiUrl}/${this.apiRoute}/${path}`, {
      params: params
    }).pipe(catchError(this.formatErrors));
  }

  find(path: string, offset: number, limit: number, params: HttpParams = new HttpParams()): Observable<PagingInfo<T>> {
    return this.http.get<T[]>(`${this.configService.config.apiUrl}/${this.apiRoute}/${path}`, {
      observe: 'response',
      params: params
    }).pipe(
      map(resp =>
        resp.headers.has('X-Total-Count')
          ? Object.assign({} as PagingInfo<T>, { totalCount: +resp.headers.get('X-Total-Count'), items: resp.body })
          : Object.assign({} as PagingInfo<T>, { totalCount: resp.body.length, items: resp.body })
      ),
      catchError(this.formatErrors)
    );
  }

  put(path: string, body: T): Observable<T> {
    return this.http.put<T>(`${this.configService.config.apiUrl}/${this.apiRoute}/${path}`, body)
      .pipe(catchError(this.formatErrors));
  }

  patch(path: string, body: T): Observable<T> {
    return this.http.patch<T>(`${this.configService.config.apiUrl}/${this.apiRoute}/${path}`, body)
      .pipe(
        catchError(this.formatErrors)
      );
  }

  post(path: string, body: T): Observable<T> {
    return this.http.post<T>(`${this.configService.config.apiUrl}/${this.apiRoute}/${path}`, body)
      .pipe(catchError(this.formatErrors));
  }

  delete(path: string): Observable<void> {
    return this.http.delete<void>(`${this.configService.config.apiUrl}/${this.apiRoute}/${path}`)
      .pipe(catchError(this.formatErrors));
  }

  private formatErrors(error: any) {
    console.error(error);
    return throwError(() => new Error(error.error));
  }
}
