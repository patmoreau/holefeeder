/* eslint-disable @typescript-eslint/no-unused-vars */
import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { Tag } from '@app/shared/models/tag.model';

// https://www.concretepage.com/ngrx/ngrx-entity-example

export const TagsActions = createActionGroup({
  source: 'Tags API',
  events: {
    loadTags: emptyProps(),
    loadTagsSuccess: props<{ tags: ReadonlyArray<Tag> }>(),
    loadTagsFailure: props<{ error: string }>(),
    clearTags: emptyProps(),
  },
});

// generated action creators:
const { loadTags, loadTagsSuccess, loadTagsFailure, clearTags } = TagsActions;
