import { Stack } from 'expo-router';
import React, { Children, isValidElement } from 'react';
import { toNativeToolbarChildren } from './AppToolbar';
import { AppToolbarButton } from './AppToolbarButton';

const typeOfFirstChild = (children: React.ReactNode) => {
  const [first] = Children.toArray(children);
  return isValidElement(first) ? first.type : undefined;
};

describe('toNativeToolbarChildren', () => {
  it('should convert a toolbar button into the element type expo-router matches on', () => {
    const children = toNativeToolbarChildren(<AppToolbarButton accessibilityLabel="Back" testID="back" onPress={jest.fn()} />);

    expect(typeOfFirstChild(children)).toBe(Stack.Toolbar.Button);
  });

  it('should carry the button props over to the converted element', () => {
    const onPress = jest.fn();

    const children = toNativeToolbarChildren(<AppToolbarButton accessibilityLabel="Save" testID="save" disabled onPress={onPress} />);

    const [first] = Children.toArray(children);
    expect(isValidElement(first) && first.props).toMatchObject({ accessibilityLabel: 'Save', disabled: true, onPress });
  });

  it('should leave other toolbar children untouched', () => {
    const children = toNativeToolbarChildren(<Stack.Toolbar.Menu icon="ellipsis" />);

    expect(typeOfFirstChild(children)).toBe(Stack.Toolbar.Menu);
  });
});
