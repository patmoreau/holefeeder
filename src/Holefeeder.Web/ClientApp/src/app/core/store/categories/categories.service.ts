import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Category, CategoryType, MessageAction, MessageType } from '@app/shared/models';
import { BehaviorSubject, catchError, map, Observable, shareReplay, throwError } from 'rxjs';
import { MessageService } from '@app/core/services';
import { Store } from '@ngrx/store';
import { AuthFeature } from '@app/core/store/auth/auth.feature';
import { filterTrue } from '@app/shared/helpers';

const apiRoute = 'categories';
const CACHE_TTL = 300000; // 5 minute cache

type categoryType = {
  id: string;
  name: string;
  type: CategoryType;
  color: string;
  budgetAmount: number;
  inactive: boolean;
};

@Injectable({ providedIn: 'root' })
export class CategoriesService {
  private categoriesSubject = new BehaviorSubject<Category[]>([]);
  private cache$: Observable<ReadonlyArray<Category>> | null = null;
  private lastFetch = 0;

  categories$ = this.categoriesSubject.asObservable();

  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private apiUrl: string,
    private messages: MessageService,
    private store: Store
  ) {
    this.store.select(AuthFeature.selectIsAuthenticated)
      .pipe(filterTrue())
      .subscribe(() => this.loadCategories());
  }

  fetch(): Observable<ReadonlyArray<Category>> {
    const now = Date.now();
    if (this.cache$ && now - this.lastFetch < CACHE_TTL) {
      return this.cache$;
    }

    this.cache$ = this.http
      .get<ReadonlyArray<categoryType>>(`${this.apiUrl}/${apiRoute}`)
      .pipe(
        map(categories => this.adaptCategories(categories)),
        catchError(error => {
          this.messages.sendMessage({
            type: MessageType.error,
            action: MessageAction.error,
            content: 'Failed to load categories. Please try again later.'
          });
          return throwError(() => error);
        }),
        shareReplay(1)
      );

    this.lastFetch = now;

    // Update the BehaviorSubject
    this.cache$.subscribe(
      categories => this.categoriesSubject.next(categories as Category[])
    );

    return this.cache$;
  }

  private loadCategories() {
    this.fetch().subscribe();
  }

  private adaptCategories(categories: ReadonlyArray<categoryType>): ReadonlyArray<Category> {
    return categories.map(c => new Category(
      c.id,
      c.name,
      c.type,
      c.color,
      c.budgetAmount,
      c.inactive
    ));
  }
}
