import { Platform, ViewStyle } from 'react-native';
import { borderRadius, componentSizes, shadows, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

/**
 * Common style utilities that can be reused across components
 * This helps reduce repetition when creating component styles
 */

/**
 * Creates a card style with consistent spacing and shadows
 */
export const createCardStyle = (theme: Theme, options?: { padding?: keyof typeof spacing; radius?: keyof typeof borderRadius }): ViewStyle => ({
  backgroundColor: theme.colors.secondaryBackground,
  borderRadius: borderRadius[options?.radius ?? 'xl'],
  padding: spacing[options?.padding ?? 'lg'],
  ...shadows.base,
  shadowColor: theme.colors.text,
});

/**
 * Creates a section container style
 */
export const createSectionStyle = (options?: {
  gap?: keyof typeof spacing;
  margin?: keyof typeof spacing;
  padding?: keyof typeof spacing;
}): ViewStyle => ({
  flexGrow: 1,
  gap: spacing[options?.gap ?? 'xs'],
  marginHorizontal: spacing[options?.margin ?? 'lg'],
  marginBottom: spacing[options?.margin ?? 'lg'],
  paddingHorizontal: spacing[options?.padding ?? 'lg'],
  borderRadius: borderRadius['3xl'],
});

/**
 * Creates a button style
 */
export const createButtonStyle = (
  type: 'primary' | 'secondary' | 'destructive' | 'link',
  theme: Theme,
  size: 'sm' | 'md' | 'lg' = 'md'
): ViewStyle => {
  const baseStyle: ViewStyle = {
    minHeight: componentSizes.button[size],
    paddingHorizontal: spacing.lg,
    paddingVertical: spacing.md,
    borderRadius: borderRadius.lg,
    alignItems: 'center',
    justifyContent: 'center',
  };

  switch (type) {
    case 'primary':
      return {
        ...baseStyle,
        backgroundColor: theme.colors.primary,
        ...shadows.sm,
      };
    case 'secondary':
      return {
        ...baseStyle,
        backgroundColor: 'transparent',
        borderWidth: 1,
        borderColor: theme.colors.primary,
      };
    case 'destructive':
      return {
        ...baseStyle,
        backgroundColor: theme.colors.destructive,
        ...shadows.sm,
      };
    case 'link':
      return {
        ...baseStyle,
        backgroundColor: 'transparent',
      };
  }
};

/**
 * Creates an input/picker style
 */
export const createInputStyle = (theme: Theme, options?: { height?: keyof typeof componentSizes.input }): ViewStyle => ({
  borderWidth: 1,
  borderColor: theme.colors.separator,
  borderRadius: borderRadius.md,
  paddingHorizontal: Platform.select({ web: spacing.xs, default: spacing.sm }),
  minHeight: componentSizes.input[options?.height ?? 'md'],
  width: '100%',
  backgroundColor: theme.colors.background,
});

/**
 * Platform-specific padding helper
 */
export const platformPadding = (iosValue?: keyof typeof spacing, androidValue?: keyof typeof spacing, webValue?: keyof typeof spacing) =>
  Platform.select({
    ios: spacing[iosValue ?? 'lg'],
    android: spacing[androidValue ?? iosValue ?? 'lg'],
    web: spacing[webValue ?? iosValue ?? 'lg'],
  });

/**
 * Platform-specific font size helper
 */
export const platformSpacing = {
  // Page-level spacing
  page: Platform.select({
    ios: spacing.lg,
    android: spacing.lg,
    web: spacing.xl,
  }),

  // Section spacing
  section: Platform.select({
    ios: spacing.md,
    android: spacing.md,
    web: spacing.lg,
  }),

  // Card spacing
  card: Platform.select({
    ios: spacing.lg,
    android: spacing.lg,
    web: spacing.lg,
  }),
};
