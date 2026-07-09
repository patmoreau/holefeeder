import React from 'react';
import { ViewProps } from 'react-native';
import { AppView } from '@/shared/presentation/AppView';
import { AppText, ThemedTextProps } from '@/shared/presentation/components/AppText';
import { useStyles } from '@/shared/theme/core/use-styles';
import { spacing } from '@/types/theme/design-tokens';

type ScreenTitleProps = {
  title: string;
  viewProps?: ViewProps;
  textProps?: ThemedTextProps;
};

const createStyles = () => ({
  titleContainer: {
    flexDirection: 'row' as const,
    alignItems: 'center' as const,
    gap: spacing.lg,
    padding: spacing['3xl'],
  },
});

export const ScreenTitle = ({ title, viewProps, textProps }: ScreenTitleProps) => {
  const styles = useStyles(createStyles);

  return (
    <AppView style={styles.titleContainer} {...viewProps}>
      <AppText variant="title" {...textProps}>
        {title}
      </AppText>
    </AppView>
  );
};
