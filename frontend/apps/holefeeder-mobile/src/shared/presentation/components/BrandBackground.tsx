import React from 'react';
import { StyleSheet, View } from 'react-native';
import { useTheme } from '@/shared/theme/core/use-theme';

export type BrandBackgroundProps = {
  children: React.ReactNode;
  testID?: string;
};

export const BrandBackground = ({ children, testID }: BrandBackgroundProps) => {
  const { theme, isDark } = useTheme();
  const wash = isDark ? 'rgba(255, 141, 40, 0.18)' : 'rgba(255, 141, 40, 0.12)';

  return (
    <View style={[styles.screen, { backgroundColor: theme.colors.secondaryBackground }]} testID={testID}>
      <View
        pointerEvents="none"
        style={[styles.wash, { experimental_backgroundImage: `linear-gradient(to bottom, ${wash}, rgba(255, 141, 40, 0))` }]}
      />
      {children}
    </View>
  );
};

const styles = StyleSheet.create({
  screen: { flex: 1 },
  wash: { position: 'absolute', top: 0, left: 0, right: 0, height: '60%' },
});
