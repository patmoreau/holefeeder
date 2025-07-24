import { Inject, Injectable, OnDestroy } from '@angular/core';
import { catchError, Observable, BehaviorSubject, throwError, shareReplay, map, takeUntil, Subject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Tag } from '@app/shared/models/tag.model';
import { MessageService } from '@app/core/services';
import { MessageType, MessageAction } from '@app/shared/models';
import { Store } from '@ngrx/store';
import { AuthFeature } from '@app/core/store/auth/auth.feature';
import { filterTrue } from '@app/shared/helpers';

const apiRoute = 'tags';
const CACHE_TTL = 300000; // 5 minute cache

@Injectable({ providedIn: 'root' })
export class TagsService implements OnDestroy {
  private tagsSubject = new BehaviorSubject<ReadonlyArray<Tag>>([]);
  private cache$: Observable<ReadonlyArray<Tag>> | null = null;
  private lastFetch = 0;
  private readonly destroy$ = new Subject<void>();

  tags$ = this.tagsSubject.asObservable();

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
    this.tagsSubject.complete();
    this.cache$ = null;
  }

  private initializeSubscriptions(): void {
    this.store.select(AuthFeature.selectIsAuthenticated)
      .pipe(
        filterTrue(),
        takeUntil(this.destroy$)
      )
      .subscribe(() => this.loadTags());
  }

  public fetch(): Observable<ReadonlyArray<Tag>> {
    const now = Date.now();
    if (this.cache$ && now - this.lastFetch < CACHE_TTL) {
      return this.cache$;
    }

    this.cache$ = this.http
      .get<ReadonlyArray<Tag>>(`${this.apiUrl}/${apiRoute}`)
      .pipe(
        map(tags => this.adaptTags(tags)),
        catchError(error => {
          console.error('HTTP error in fetch tags:', error);
          this.messages.sendMessage({
            type: MessageType.error,
            action: MessageAction.error,
            content: 'Failed to load tags. Please try again later.'
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
        next: tags => this.tagsSubject.next(tags),
        error: error => {
          console.error('Error updating tags subject:', error);
        }
      });

    return this.cache$;
  }

  private loadTags() {
    this.fetch()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          // Tags are already updated via the fetch method
        },
        error: error => {
          console.error('Error in loadTags:', error);
        }
      });
  }

  private adaptTags(tags: ReadonlyArray<Tag>): ReadonlyArray<Tag> {
    return tags.map(t => new Tag(t.tag, t.count));
  }
}
