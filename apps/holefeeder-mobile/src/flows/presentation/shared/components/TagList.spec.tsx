import { act, fireEvent, render, screen } from '@testing-library/react-native';
import React, { useState } from 'react';
import { aTag } from '@/flows/core/flows/__tests__/tag-for-test';
import { Tag } from '@/flows/core/flows/tag';
import { TagList } from '@/flows/presentation/shared/components/TagList';
import { anId } from '@/shared/__tests__/string-for-test';
import { aLightThemeState } from '@/shared/theme/__tests__/theme-state-for-test';
import { ThemeProviderForTest } from '@/shared/theme/__tests__/ThemeProviderForTest';

const placeHolderText = 'tagList.placeHolder';
const categoryId = anId();
const firstTag = aTag({ tag: 'first-tag', count: 1, categoryId });
const middleTag = aTag({ tag: 'middle-tag', count: 2, categoryId });
const lastTag = aTag({ tag: 'last-tag', count: 10, categoryId: anId() });
const selectedTag = aTag({ tag: 'selected-tag', count: 4, categoryId: anId() });
const newTag = aTag({ tag: 'new-tag', count: 0, categoryId: undefined });
const tagsPattern = new RegExp(`^#(?:${firstTag.tag}|${middleTag.tag}|${lastTag.tag}|${selectedTag.tag}|${newTag.tag})$`);

function TestHost() {
  const [selected, setSelected] = useState<Tag[]>([selectedTag]);
  return <TagList tags={[firstTag, middleTag, lastTag, selectedTag]} selected={selected} onChange={setSelected} categoryId={categoryId} />;
}

const displayTag = (tag: Tag) => `#${tag.tag}`;

describe('TagList', () => {
  const themeState = aLightThemeState();

  beforeEach(() => {
    render(
      <ThemeProviderForTest overrides={themeState}>
        <TestHost />
      </ThemeProviderForTest>
    );
  });

  it('shows placeholder', () => {
    expect(screen.queryByPlaceholderText(placeHolderText)).toBeOnTheScreen();
  });

  it('shows tags in order', () => {
    const tags = screen.queryAllByTestId(tagsPattern);

    expect(tags).toHaveLength(4);
    expect(tags.map((tag) => tag.props.children)).toEqual([
      displayTag(selectedTag),
      displayTag(middleTag),
      displayTag(firstTag),
      displayTag(lastTag),
    ]);
  });

  describe('when toggling a tag', () => {
    it('toggle to selected on pressing an unselected tag', () => {
      act(() => fireEvent.press(screen.getByTestId(displayTag(middleTag))));

      const tags = screen.queryAllByTestId(tagsPattern);

      expect(tags).toHaveLength(4);
      expect(tags.map((tag) => tag.props.children)).toEqual([
        displayTag(selectedTag),
        displayTag(middleTag),
        displayTag(firstTag),
        displayTag(lastTag),
      ]);
    });

    it('toggle to unselected on pressing a selected tag', () => {
      act(() => fireEvent.press(screen.getByTestId(displayTag(selectedTag))));

      const tags = screen.queryAllByTestId(tagsPattern);

      expect(tags).toHaveLength(4);
      expect(tags.map((tag) => tag.props.children)).toEqual([
        displayTag(middleTag),
        displayTag(firstTag),
        displayTag(lastTag),
        displayTag(selectedTag),
      ]);
    });
  });

  describe('when entering text', () => {
    it('shows tags matching the text', () => {
      act(() => fireEvent.changeText(screen.getByPlaceholderText(placeHolderText), 'd'));

      const tags = screen.queryAllByTestId(tagsPattern);

      expect(tags).toHaveLength(2);
      expect(tags.map((tag) => tag.props.children)).toEqual([displayTag(selectedTag), displayTag(middleTag)]);
    });

    it('trim spaces', () => {
      act(() => fireEvent.changeText(screen.getByPlaceholderText(placeHolderText), ' d '));

      const tags = screen.queryAllByTestId(tagsPattern);

      expect(tags).toHaveLength(2);
      expect(tags.map((tag) => tag.props.children)).toEqual([displayTag(selectedTag), displayTag(middleTag)]);
    });

    it('shows no tags on no match', () => {
      act(() => fireEvent.changeText(screen.getByPlaceholderText(placeHolderText), 'z'));

      const tags = screen.queryAllByTestId(tagsPattern);

      expect(tags.length).toBe(0);
    });

    it('on enter with exact single match, selects that tag and clears filter', () => {
      const input = screen.getByPlaceholderText(placeHolderText);
      const add = screen.getByTestId('image-plus');

      act(() => fireEvent.changeText(input, 'mid'));
      act(() => fireEvent.press(add));

      const tags = screen.queryAllByTestId(tagsPattern);

      expect(tags).toHaveLength(4);
      expect(tags.map((tag) => tag.props.children)).toEqual([
        displayTag(middleTag),
        displayTag(selectedTag),
        displayTag(firstTag),
        displayTag(lastTag),
      ]);
    });

    it('on pressing a tag, selects that tag and clears filter', () => {
      const input = screen.getByPlaceholderText(placeHolderText);

      act(() => fireEvent.changeText(input, 'mid'));
      act(() => fireEvent.press(screen.getByTestId(displayTag(middleTag))));

      const tags = screen.queryAllByTestId(tagsPattern);

      expect(tags).toHaveLength(4);
      expect(tags.map((tag) => tag.props.children)).toEqual([
        displayTag(selectedTag),
        displayTag(middleTag),
        displayTag(firstTag),
        displayTag(lastTag),
      ]);
    });

    it('on enter with multiple matches, creates a new lowercase tag and selects it, then clears filter', () => {
      const input = screen.getByPlaceholderText(placeHolderText);
      const add = screen.getByTestId('image-plus');

      act(() => fireEvent.changeText(input, newTag.tag.toUpperCase()));
      act(() => fireEvent.press(add));

      const tags = screen.queryAllByTestId(tagsPattern);

      expect(tags).toHaveLength(5);
      expect(tags.map((tag) => tag.props.children)).toEqual([
        displayTag(newTag),
        displayTag(selectedTag),
        displayTag(middleTag),
        displayTag(firstTag),
        displayTag(lastTag),
      ]);
    });

    it('on enter, add new tag to list and selects it', () => {
      const input = screen.getByPlaceholderText(placeHolderText);
      const add = screen.getByTestId('image-plus');

      act(() => fireEvent.changeText(input, newTag.tag));
      act(() => fireEvent.press(add));

      const tags = screen.queryAllByTestId(tagsPattern);

      expect(tags).toHaveLength(5);
      expect(tags.map((tag) => tag.props.children)).toEqual([
        displayTag(newTag),
        displayTag(selectedTag),
        displayTag(middleTag),
        displayTag(firstTag),
        displayTag(lastTag),
      ]);
    });
  });
});
