import { render } from '@testing-library/react-native';
import React from 'react';
import { Text, View } from 'react-native';
import { AppSection } from './AppSection';

// Mock the useStyles hook
jest.mock('@/shared/theme/core/use-styles', () => ({
  useStyles: jest.fn(() => ({
    section: {
      padding: 16,
      backgroundColor: '#fff',
    },
    divider: {
      height: 1,
      backgroundColor: '#e0e0e0',
      marginVertical: 8,
    },
  })),
}));

describe('Section', () => {
  it('renders children without dividers when there is only one child', () => {
    const { getByText } = render(
      <AppSection>
        <Text>Child 1</Text>
      </AppSection>
    );

    expect(getByText('Child 1')).toBeTruthy();
  });

  it('renders dividers between multiple children', () => {
    const { getByText, UNSAFE_root } = render(
      <AppSection>
        <View testID="child1">
          <Text>Child 1</Text>
        </View>
        <View testID="child2">
          <Text>Child 2</Text>
        </View>
        <View testID="child3">
          <Text>Child 3</Text>
        </View>
      </AppSection>
    );

    expect(getByText('Child 1')).toBeTruthy();
    expect(getByText('Child 2')).toBeTruthy();
    expect(getByText('Child 3')).toBeTruthy();

    // Check that dividers are rendered (2 dividers for 3 children)
    const allViews = UNSAFE_root.findAllByType(View);
    // Should have: Section container + 3 child Views + 2 divider Views = 6 Views
    expect(allViews.length).toBe(7);
  });

  it('does not render a divider after the last child', () => {
    const { UNSAFE_root } = render(
      <AppSection>
        <View testID="first">
          <Text>First</Text>
        </View>
        <View testID="last">
          <Text>Last</Text>
        </View>
      </AppSection>
    );

    const allViews = UNSAFE_root.findAllByType(View);
    // Should have: Section container + 2 child Views + 1 divider View = 4 Views
    expect(allViews.length).toBe(5);
  });
});
