import { render, screen } from '@testing-library/react-native';
import React from 'react';
import { BrandTile } from '@/shared/presentation/components/BrandTile';
import { aDarkThemeState, aLightThemeState } from '@/shared/theme/__tests__/theme-state-for-test';
import { ThemeProviderForTest } from '@/shared/theme/__tests__/ThemeProviderForTest';
const darkTile = require('../../../../assets/images/brand-tile-dark.png');
const lightTile = require('../../../../assets/images/brand-tile.png');

describe('BrandTile', () => {
  it('shows the light tile in light mode', async () => {
    await render(
      <ThemeProviderForTest overrides={aLightThemeState()}>
        <BrandTile testID="brand-tile" />
      </ThemeProviderForTest>
    );

    expect(screen.getByTestId('brand-tile').props.source).toEqual([lightTile]);
  });

  it('shows the dark tile in dark mode', async () => {
    await render(
      <ThemeProviderForTest overrides={aDarkThemeState()}>
        <BrandTile testID="brand-tile" />
      </ThemeProviderForTest>
    );

    expect(screen.getByTestId('brand-tile').props.source).toEqual([darkTile]);
  });
});
