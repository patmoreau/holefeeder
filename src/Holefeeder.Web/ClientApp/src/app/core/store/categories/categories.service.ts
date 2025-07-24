import { HttpClient } from '@angular/common/http';
import { Inject, Injectable, OnDestroy } from '@angular/core';
import { Category, CategoryType, MessageAction, MessageType } from '@app/shared/models';
import { BehaviorSubject, catchError, map, Observable, shareReplay, throwError, takeUntil, Subject } from 'rxjs';
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
export class CategoriesService implements OnDestroy {
  private categoriesSubject = new BehaviorSubject<Category[]>([]);
  private cache$: Observable<ReadonlyArray<Category>> | null = null;
  private lastFetch = 0;
  private readonly destroy$ = new Subject<void>();

  categories$ = this.categoriesSubject.asObservable();

  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private apiUrl: string,
    private messages: MessageService,
    private store: Store
  ) {
    this.initializeSubscriptions();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.categoriesSubject.complete();
    this.cache$ = null;
  }

  private initializeSubscriptions(): void {
    this.store.select(AuthFeature.selectIsAuthenticated)
      .pipe(
        filterTrue(),
        takeUntil(this.destroy$)
      )
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
          console.error('HTTP error in fetch categories:', error);
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

    // Update the BehaviorSubject with proper subscription management
    this.cache$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: categories => this.categoriesSubject.next(categories as Category[]),
        error: error => {
          console.error('Error updating categories subject:', error);
        }
      });

    return this.cache$;
  }

  private loadCategories() {
    this.fetch()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          // Categories are already updated via the fetch method
        },
        error: error => {
          console.error('Error in loadCategories:', error);
        }
      });
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
