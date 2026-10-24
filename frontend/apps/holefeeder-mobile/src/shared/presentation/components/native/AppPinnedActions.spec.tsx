import { buttonStyle, frame } from '@expo/ui/swift-ui/modifiers';
import { render } from '@testing-library/react-native';
import React from 'react';
import { AppPinnedActions } from '@/shared/presentation/components/native/AppPinnedActions';
import { aLightThemeState } from '@/shared/theme/__tests__/theme-state-for-test';
import { ThemeProviderForTest } from '@/shared/theme/__tests__/ThemeProviderForTest';

jest.mock('react-native-safe-area-context', () => ({
  useSafeAreaInsets: () => ({ top: 0, bottom: 34, left: 0, right: 0 }),
}));

const renderActions = () =>
  render(
    <ThemeProviderForTest overrides={aLightThemeState()}>
      <AppPinnedActions primary={{ label: 'Primary', onPress: jest.fn() }} secondary={{ label: 'Secondary', onPress: jest.fn() }} />
    </ThemeProviderForTest>
  );

describe('AppPinnedActions', () => {
  beforeEach(() => {
    jest.mocked(buttonStyle).mockClear();
    jest.mocked(frame).mockClear();
  });

  it('draws the secondary action as a full-width glass button', async () => {
    await renderActions();

    expect(buttonStyle).toHaveBeenCalledWith('glass');
    expect(frame).toHaveBeenCalledWith({ maxWidth: Infinity });
  });
});
