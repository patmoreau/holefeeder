import { Observable, of, take, throwError } from 'rxjs';
import { TagsActions } from '../tags.actions';
import { TestBed } from '@angular/core/testing';
import { Action } from '@ngrx/store';
import { fetchTags } from '../tags.effects';
import { TagsService } from '../tags.service';
import { provideMockActions } from '@ngrx/effects/testing';
import { Tag } from '@app/shared/models/tag.model';

describe('Tags Effects', (): void => {
  let tagsServiceMock: TagsService;
  let actions$: Observable<Action>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideMockActions(() => actions$),
        { provide: TagsService, useValue: tagsServiceMock }, // Provide the mock TagsService
      ],
    });
  });

  it('should dispatch loadTagsSuccess action on successful fetch', done => {
    const mockTags: ReadonlyArray<Tag> = [
      {
        tag: 'tag#1',
        count: 12,
      },
    ];
    tagsServiceMock = {
      fetch: () => of(mockTags),
    } as unknown as TagsService;

    actions$ = of(TagsActions.loadTags());

    TestBed.runInInjectionContext((): void => {
      fetchTags(actions$, tagsServiceMock)
        .pipe(take(1))
        .subscribe(action => {
          expect(action).toEqual(
            TagsActions.loadTagsSuccess({
              tags: mockTags,
            })
          );
          done();
        });
    });
  });

  it('should dispatch loadTagsFailure action when tags fetch failed', done => {
    tagsServiceMock = {
      fetch: () => throwError(() => new Error('Error message')),
    } as unknown as TagsService;

    actions$ = of(TagsActions.loadTags());

    TestBed.runInInjectionContext((): void => {
      fetchTags(actions$, tagsServiceMock)
        .pipe(take(1))
        .subscribe(action => {
          expect(action).toEqual(
            TagsActions.loadTagsFailure({
              error: 'Error message',
            })
          );
          done();
        });
    });
  });
});
