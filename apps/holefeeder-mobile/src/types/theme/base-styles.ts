import { Platform, TextStyle, ViewStyle } from 'react-native';
import { borderRadius, componentSizes, shadows, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

/**
 * Base style factory - creates reusable styles that use design tokens
 * This reduces repetition across the codebase
 */
export const createBaseStyles = (theme: Theme) => ({
  // Container styles
  containers: {
    page: {
      flex: 1,
      padding: spacing.lg,
    } as ViewStyle,

    center: {
      flex: 1,
      alignItems: 'center',
      justifyContent: 'center',
    } as ViewStyle,

    section: {
      flexGrow: 1,
      gap: spacing.xs,
      marginHorizontal: spacing.lg,
      marginBottom: spacing.lg,
      paddingHorizontal: spacing.lg,
      borderRadius: borderRadius['3xl'],
    } as ViewStyle,

    card: {
      backgroundColor: theme.colors.secondaryBackground,
      borderRadius: borderRadius.xl,
      padding: spacing.lg,
      ...shadows.base,
      shadowColor: theme.colors.text,
    } as ViewStyle,
  },

  // Button styles
  buttons: {
    base: {
      minHeight: componentSizes.button.md,
      paddingHorizontal: spacing.lg,
      paddingVertical: spacing.md,
      borderRadius: borderRadius.lg,
      alignItems: 'center',
      justifyContent: 'center',
      ...shadows.sm,
    } as ViewStyle,

    primary: {
      minHeight: componentSizes.button.md,
      paddingHorizontal: spacing.lg,
      paddingVertical: spacing.md,
      borderRadius: borderRadius.lg,
      alignItems: 'center',
      justifyContent: 'center',
      backgroundColor: theme.colors.primary,
      ...shadows.sm,
    } as ViewStyle,

    secondary: {
      minHeight: componentSizes.button.md,
      paddingHorizontal: spacing.lg,
      paddingVertical: spacing.md,
      borderRadius: borderRadius.lg,
      alignItems: 'center',
      justifyContent: 'center',
      backgroundColor: 'transparent',
      borderWidth: 1,
      borderColor: theme.colors.primary,
    } as ViewStyle,

    destructive: {
      minHeight: componentSizes.button.md,
      paddingHorizontal: spacing.lg,
      paddingVertical: spacing.md,
      borderRadius: borderRadius.lg,
      alignItems: 'center',
      justifyContent: 'center',
      backgroundColor: theme.colors.destructive,
      ...shadows.sm,
    } as ViewStyle,

    link: {
      minHeight: componentSizes.button.md,
      paddingHorizontal: spacing.lg,
      paddingVertical: spacing.md,
      alignItems: 'center',
      justifyContent: 'center',
      backgroundColor: 'transparent',
    } as ViewStyle,
  },

  // Input/Picker styles
  inputs: {
    base: {
      borderWidth: 1,
      borderColor: theme.colors.separator,
      borderRadius: borderRadius.md,
      paddingHorizontal: Platform.select({ web: spacing.xs, default: spacing.sm }),
      minHeight: componentSizes.input.md,
      width: '100%',
      backgroundColor: theme.colors.background,
      color: theme.colors.text,
    } as TextStyle,

    picker: {
      borderWidth: 1,
      borderColor: theme.colors.separator,
      borderRadius: borderRadius.md,
      paddingHorizontal: Platform.select({ web: spacing.xs, default: spacing.sm }),
      minHeight: componentSizes.input.md,
      width: '100%',
      backgroundColor: theme.colors.background,
      color: theme.colors.text,
      ...(Platform.OS === 'web' && {
        appearance: 'none' as any,
        backgroundImage:
          'url("data:image/svg+xml;charset=US-ASCII,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20width%3D%22292.4%22%20height%3D%22292.4%22%3E%3Cpath%20fill%3D%22%23007CB2%22%20d%3D%22M287%2069.4a17.6%2017.6%200%200%200-13-5.4H18.4c-5%200-9.3%201.8-12.9%205.4A17.6%2017.6%200%200%200%200%2082.2c0%205%201.8%209.3%205.4%2012.9l128%20127.9c3.6%203.6%207.8%205.4%2012.8%205.4s9.2-1.8%2012.8-5.4L287%2095c3.5-3.5%205.4-7.8%205.4-12.8%200-5-1.9-9.2-5.4-12.8z%22%2F%3E%3C%2Fsvg%3E")',
        backgroundRepeat: 'no-repeat',
        backgroundPosition: 'right 0.7rem top 50%',
        backgroundSize: '0.65rem auto',
      }),
    } as TextStyle,
  },

  // Text styles
  text: {
    link: {
      color: theme.colors.link,
      textDecorationLine: 'underline',
    } as TextStyle,
  },

  // Layout helpers
  layout: {
    row: {
      flexDirection: 'row',
    } as ViewStyle,

    column: {
      flexDirection: 'column',
    } as ViewStyle,

    center: {
      alignItems: 'center',
      justifyContent: 'center',
    } as ViewStyle,
  },
});
