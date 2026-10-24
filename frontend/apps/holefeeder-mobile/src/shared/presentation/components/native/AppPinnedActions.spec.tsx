import { buttonStyle, frame } from '@expo/ui/swift-ui/modifiers';
import { render } from '@testing-library/react-native';
import React from 'react';
import { AppPinnedActions } from '@/shared/presentation/components/native/AppPinnedActions';
import { aLightThemeState } from '@/shared/theme/__tests__/theme-state-for-test';
import { ThemeProviderForTest } from '@/shared/theme/__tests__/ThemeProviderForTest';

jest.mock('react-native-safe-area-context', () => ({
  useSafeAreaInsets: () => ({ top: 0, bottom: 34, left: 0, right: 0 }),
}));

const renderActions = (secondaryStyle?: 'link' | 'button') =>
  render(
    <ThemeProviderForTest overrides={aLightThemeState()}>
      <AppPinnedActions
        primary={{ label: 'Primary', onPress: jest.fn() }}
        secondary={{ label: 'Secondary', onPress: jest.fn() }}
        secondaryStyle={secondaryStyle}
      />
    </ThemeProviderForTest>
  );

describe('AppPinnedActions', () => {
  beforeEach(() => {
    jest.mocked(buttonStyle).mockClear();
    jest.mocked(frame).mockClear();
  });

  it('draws the secondary action as a full-width glass button by default', async () => {
    await renderActions();

    expect(buttonStyle).toHaveBeenCalledWith('glass');
    expect(buttonStyle).not.toHaveBeenCalledWith('plain');
    expect(frame).toHaveBeenCalledTimes(2);
  });

  it('draws the secondary action as a plain link when asked', async () => {
    await renderActions('link');

    expect(buttonStyle).toHaveBeenCalledWith('plain');
    expect(buttonStyle).not.toHaveBeenCalledWith('glass');
  });

  it('draws the secondary action as a full-width glass button when asked', async () => {
    await renderActions('button');

    expect(buttonStyle).toHaveBeenCalledWith('glass');
    expect(buttonStyle).not.toHaveBeenCalledWith('plain');
    expect(frame).toHaveBeenCalledTimes(2);
  });
});
