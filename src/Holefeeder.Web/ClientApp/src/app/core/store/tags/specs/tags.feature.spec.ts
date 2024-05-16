import { TagsActions } from '../tags.actions';
import { Tag } from '@app/shared/models';
import {
  TagsFeature,
  TagsState,
  initialTagsState,
} from '../tags.feature';

describe('Tags feature', () => {
  it('[Tags API] Load Tags', () => {
    // arrange
    const action = TagsActions.loadTags();

    const expectedState: TagsState = {
      ...initialTagsState,
      callState: 'loading',
    };

    // act
    const result = TagsFeature.reducer(initialTagsState, action);

    // assert
    expect(result).toEqual(expectedState);
  });

  it('[Tags API] Load Tags Success', () => {
    // arrange
    const tag = new Tag(
      'Tag 1',
      123,
    );
    const tags = [tag];
    const action = TagsActions.loadTagsSuccess({ tags });

    const expectedState: TagsState = {
      ...initialTagsState,
      tags,
      callState: 'loaded',
    };

    // act
    const result = TagsFeature.reducer(initialTagsState, action);

    // assert
    expect(result).toEqual(expectedState);
  });

  it('[Tags API] Load Tags Failure', () => {
    // arrange
    const error = 'error';
    const action = TagsActions.loadTagsFailure({ error });

    const expectedState: TagsState = {
      ...initialTagsState,
      error,
      callState: 'loaded',
    };

    // act
    const result = TagsFeature.reducer(initialTagsState, action);

    // assert
    expect(result).toEqual(expectedState);
  });

  it('[Tags API] Clear Tags', () => {
    // arrange
    const tag = new Tag(
      'Tag 1',
      123,
    );
    const tags = [tag];

    const state: TagsState = {
      ...initialTagsState,
      tags,
      callState: 'loaded',
    };
    const action = TagsActions.clearTags();

    const expectedState: TagsState = {
      ...initialTagsState,
      callState: 'init',
    };

    // act
    const result = TagsFeature.reducer(state, action);

    // assert
    expect(result).toEqual(expectedState);
  });
});
