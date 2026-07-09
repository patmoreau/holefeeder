import React from 'react';
import { View, type ViewProps } from 'react-native';
import { useStyles } from '@/shared/theme/core/use-styles';
import { Theme } from '@/types/theme';

export type AppCardDividerProps = ViewProps & {};

const createStyles = (theme: Theme) => ({
  divider: {
    height: 1,
    backgroundColor: theme.colors.separator,
  },
});

export const AppCardDivider = ({ style, ...props }: AppCardDividerProps) => {
  const styles = useStyles(createStyles);

  return <View style={[styles.divider, style]} {...props} />;
};
