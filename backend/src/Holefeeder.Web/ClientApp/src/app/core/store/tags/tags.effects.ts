import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of } from 'rxjs';
import { TagsService } from './tags.service';
import { TagsActions } from './tags.actions';

export const fetchTags = createEffect(
  (
    actions$ = inject(Actions),
    service = inject(TagsService)
  ) => {
    return actions$.pipe(
      ofType(TagsActions.loadTags),
      exhaustMap(() =>
        service.fetch().pipe(
          map(tags =>
            TagsActions.loadTagsSuccess({ tags: tags })
          ),
          catchError(error =>
            of(
              TagsActions.loadTagsFailure({ error: error.message })
            )
          )
        )
      )
    );
  },
  { functional: true, dispatch: true }
);
