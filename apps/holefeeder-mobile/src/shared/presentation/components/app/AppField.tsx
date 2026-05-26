import { Icon } from '@expo/ui';
import React from 'react';
import { ExpoColumn } from '@/shared/presentation/components/expo/ExpoColumn';
import { ExpoIcon } from '@/shared/presentation/components/expo/ExpoIcon';
import { ExpoRow } from '@/shared/presentation/components/expo/ExpoRow';
import { ExpoSpacer } from '@/shared/presentation/components/expo/ExpoSpacer';
import { ExpoText } from '@/shared/presentation/components/expo/ExpoText';
import { AppIconsMappings } from '@/shared/presentation/core/app-icons';
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
  label?: string;
  icon: AppIconsMappings;
  children: React.ReactNode;
  variant?: 'default' | 'large';
  error?: string;
};

export const AppField = ({ label, icon, children, variant = 'default', error }: FieldProps) => {
  const styles = useStyles(createStyles);
  const { theme } = useTheme();

  if (variant === 'large') {
    return (
      <ExpoColumn spacing={8}>
        <ExpoRow spacing={8} alignment={'center'}>
          <ExpoIcon name={Icon.select(icon)} size={24} color={theme.colors.primary} style={styles.iconCircle} />
          {label && <ExpoText>{label}</ExpoText>}
        </ExpoRow>
        {children}
      </ExpoColumn>
    );
  }

  return (
    <ExpoRow spacing={8} alignment={'center'}>
      <ExpoIcon name={Icon.select(icon)} size={24} color={theme.colors.primary} style={styles.iconCircle} />
      {label && <ExpoText>{label}</ExpoText>}
      <ExpoSpacer flexible />
      {children}
    </ExpoRow>
  );
};
