import { View, ViewProps } from 'react-native';
import { useStyles } from '@/shared/theme/core/use-styles';
import { borderRadius, shadows, spacing, Theme } from '@/types/theme';

export type AppCardProps = ViewProps & ({ scrollable?: 'vertical' } | { scrollable: 'horizontal'; cardWidth: number });

const createStyles = (theme: Theme) => ({
  verticalCard: {
    flex: 1,
    flexDirection: 'row' as const,
    overflow: 'hidden' as const,
    backgroundColor: theme.colors.background,
    paddingHorizontal: spacing.md,
    paddingVertical: spacing.sm,
    ...shadows.base,
  },
  horizontalCard: {
    backgroundColor: theme.colors.secondaryBackground,
    borderRadius: borderRadius.xl,
    padding: spacing.lg,
    marginRight: spacing.lg,
    shadowColor: theme.colors.text,
    ...shadows.base,
  },
});

export const AppCard = ({ scrollable = 'vertical', style, children, ...props }: AppCardProps) => {
  const styles = useStyles(createStyles);
  if (scrollable === 'horizontal') {
    const { cardWidth, ...rest } = props as ViewProps & { scrollable: 'horizontal'; cardWidth: number };
    return (
      <View style={[styles.horizontalCard, { width: cardWidth }, style]} {...rest}>
        {children}
      </View>
    );
  }

  return (
    <View style={[styles.verticalCard, style]} {...props}>
      {children}
    </View>
  );
};
