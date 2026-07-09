import { Injectable, inject } from '@angular/core';
import { Observable, of, EMPTY } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { LoggerService } from '@app/core/logger/logger.service';

@Injectable({
  providedIn: 'root'
})
export class SafeOperationsService {
  private loggerService = inject(LoggerService);


  /**
   * Safely execute a function and return a default value if it throws
   */
  safeExecute<T>(
    fn: () => T,
    defaultValue: T,
    context?: string
  ): T {
    try {
      return fn();
    } catch (error) {
      this.loggerService.logError(error as Error, {
        component: 'SafeOperationsService',
        action: `safeExecute${context ? `: ${context}` : ''}`
      });
      return defaultValue;
    }
  }

  /**
   * Safely execute an async function
   */
  async safeExecuteAsync<T>(
    fn: () => Promise<T>,
    defaultValue: T,
    context?: string
  ): Promise<T> {
    try {
      return await fn();
    } catch (error) {
      this.loggerService.logError(error as Error, {
        component: 'SafeOperationsService',
        action: `safeExecuteAsync${context ? `: ${context}` : ''}`
      });
      return defaultValue;
    }
  }

  /**
   * Safely execute an Observable operation
   */
  safeObservable<T>(
    obs$: Observable<T>,
    fallbackValue?: T,
    context?: string
  ): Observable<T> {
    return obs$.pipe(
      catchError((error) => {
        this.loggerService.logError(error, {
          component: 'SafeOperationsService',
          action: `safeObservable${context ? `: ${context}` : ''}`
        });

        if (fallbackValue !== undefined) {
          return of(fallbackValue);
        }

        return EMPTY;
      })
    );
  }

  /**
   * Safely parse JSON
   */
  safeJsonParse<T>(
    jsonString: string,
    defaultValue: T,
    context?: string
  ): T {
    return this.safeExecute(
      () => JSON.parse(jsonString),
      defaultValue,
      context || 'JSON parse'
    );
  }

  /**
   * Safely access nested object properties
   */
  safeGet<T>(
    obj: unknown,
    path: string,
    defaultValue: T
  ): T {
    return this.safeExecute(
      () => {
        if (obj === null || obj === undefined) {
          return defaultValue;
        }

        let current: unknown = obj;
        const keys = path.split('.');

        for (const key of keys) {
          if (current === null || current === undefined) {
            return defaultValue;
          }

          if (typeof current === 'object' && current !== null) {
            current = (current as Record<string, unknown>)[key];
          } else {
            return defaultValue;
          }
        }

        return current as T ?? defaultValue;
      },
      defaultValue,
      `property access: ${path}`
    );
  }
}
