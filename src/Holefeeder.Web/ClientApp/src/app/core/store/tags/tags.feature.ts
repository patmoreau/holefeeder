import { createFeature, createReducer, createSelector, on } from '@ngrx/store';
import { TagsActions } from './tags.actions';
import { CallState } from '@app/core/store/call-state.type';
import { Tag } from '@app/shared/models/tag.model';

export interface TagsState {
  callState: CallState;
  tags: ReadonlyArray<Tag>;
  error: string;
}

export const initialTagsState: TagsState ={
  callState: 'init',
  tags: [],
  error: '',
};

export const TagsFeature = createFeature({
  name: 'tags',
  reducer: createReducer(
    initialTagsState,
    on(TagsActions.loadTags, state => ({
      ...state,
      callState: 'loading' as const,
    })),
    on(TagsActions.loadTagsSuccess, (state, { tags }) => ({
      ...state,
      tags,
      callState: 'loaded' as const,
    })),
    on(TagsActions.loadTagsFailure, (state, { error }) => ({
      ...state,
      error,
      callState: 'loaded' as const,
    })),
    on(TagsActions.clearTags, state => ({
      ...state,
      tags: [],
      callState: 'init' as const,
    }))
  ),
  extraSelectors: ({ selectTags }) => ({
    contains: (text: string) =>
      createSelector(
        selectTags,
        tags => tags.filter(t => t?.tag.toLowerCase().includes(text.toLowerCase()))
      ),
  }),
});
