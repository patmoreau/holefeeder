import { Image } from 'expo-image';
import React from 'react';
import { StyleSheet } from 'react-native';
import { AppReact } from '@/shared/presentation/components/native/AppReact';
import { useTheme } from '@/shared/theme/core/use-theme';
const darkTile = require('../../../../assets/images/brand-tile-dark.png');
const lightTile = require('../../../../assets/images/brand-tile.png');

export type BrandTileProps = {
  testID?: string;
};

export const BrandTile = ({ testID }: BrandTileProps) => {
  const { isDark } = useTheme();

  return (
    <AppReact matchContents>
      <Image source={isDark ? darkTile : lightTile} style={styles.tile} accessible={false} testID={testID} />
    </AppReact>
  );
};

const styles = StyleSheet.create({
  tile: { width: 180, height: 180 },
});
