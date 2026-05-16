import React from 'react';
import { type StyleProp, Text, View, type ViewProps, type ViewStyle } from 'react-native';
import { AppText } from '@/shared/presentation/components/AppText';
import { IconSymbol } from '@/shared/presentation/components/IconSymbol';
import { AppIcons } from '@/shared/presentation/icons';
import { useStyles } from '@/shared/theme/core/use-styles';
import { useTheme } from '@/shared/theme/core/use-theme';
import { borderRadius, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  container: {
    paddingVertical: spacing.sm,
  },
  defaultContainer: {
    flexDirection: 'row' as const,
    alignItems: 'center' as const,
    gap: spacing.sm,
  },
  largeContainer: {
    flexDirection: 'column' as const,
    gap: spacing.sm,
  },
  iconCircle: {
    width: 36,
    height: 36,
    borderRadius: borderRadius.xl,
    backgroundColor: `${theme.colors.primary}20` as const,
    justifyContent: 'center' as const,
    alignItems: 'center' as const,
  },
  label: {
    ...theme.typography.body,
    color: theme.colors.text,
    flexShrink: 0,
  },
  defaultContent: {
    flex: 1,
    width: '100%' as const,
    alignItems: 'flex-end' as const,
    justifyContent: 'flex-end' as const,
    overflow: 'hidden' as const,
  },
  largeContent: {
    flex: 1,
    width: '100%' as const,
  },
});

export type FieldProps = {
  label?: string | React.ReactNode;
  icon: AppIcons;
  children: React.ReactNode;
  style?: StyleProp<ViewStyle>;
  variant?: 'default' | 'large';
  error?: string;
} & Omit<ViewProps, 'children'>;

export const AppField = ({ label, icon, children, style, variant = 'default', error, ...otherProps }: FieldProps) => {
  const styles = useStyles(createStyles);
  const { theme } = useTheme();

  if (variant === 'large') {
    return (
      <View style={[styles.container, style]} {...otherProps}>
        <View style={styles.largeContainer}>
          <View style={styles.defaultContainer}>
            <View style={styles.iconCircle}>
              <IconSymbol name={icon} size={24} color={theme.colors.primary} />
            </View>
            {label && <AppText variant={'default'}>{label}</AppText>}
          </View>
          <View style={styles.largeContent}>{children}</View>
        </View>
        {error && <AppText variant={'errorField'}>{error}</AppText>}
      </View>
    );
  }

  return (
    <View style={[styles.container, style]} {...otherProps}>
      <View style={styles.defaultContainer}>
        <View style={styles.iconCircle}>
          <IconSymbol name={icon} size={24} color={theme.colors.primary} />
        </View>
        {label && <Text style={styles.label}>{label}</Text>}
        <View style={styles.defaultContent}>{children}</View>
      </View>
      {error && <AppText variant={'errorField'}>{error}</AppText>}
    </View>
  );
};
