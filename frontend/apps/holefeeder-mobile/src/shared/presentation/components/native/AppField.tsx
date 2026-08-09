import { Icon } from '@expo/ui';
import React, { ReactNode } from 'react';
import { AppModifiers } from '@/shared/presentation/components/native/AppModifiers';
import { ExpoColumn } from '@/shared/presentation/components/native/expo/ExpoColumn';
import { ExpoIcon } from '@/shared/presentation/components/native/expo/ExpoIcon';
import { ExpoRow } from '@/shared/presentation/components/native/expo/ExpoRow';
import { ExpoSpacer } from '@/shared/presentation/components/native/expo/ExpoSpacer';
import { ExpoText } from '@/shared/presentation/components/native/expo/ExpoText';
import { UniversalIcon } from '@/shared/presentation/core/app-icon-map';
import { useStyles } from '@/shared/theme/core/use-styles';
import { useTheme } from '@/shared/theme/core/use-theme';
import { borderRadius } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  iconCircle: {
    width: 36,
    height: 36,
    borderRadius: borderRadius.xl,
    backgroundColor: `${theme.colors.primary}20` as const,
    justifyContent: 'center' as const,
    alignItems: 'center' as const,
  },
});

export type FieldProps = {
  label?: string;
  icon: UniversalIcon;
  children: ReactNode;
  variant?: 'default' | 'large';
  error?: string;
  testID?: string;
};

export const AppField = ({ label, icon, children, variant = 'default', testID }: FieldProps) => {
  const styles = useStyles(createStyles);
  const { theme } = useTheme();

  if (variant === 'large') {
    return (
      <ExpoColumn spacing={8} testID={testID}>
        <ExpoRow spacing={8} alignment={'center'}>
          <ExpoIcon name={Icon.select(icon)} size={24} color={theme.colors.primary} style={styles.iconCircle} />
          {label && <ExpoText>{label}</ExpoText>}
        </ExpoRow>
        <ExpoRow modifiers={[AppModifiers.fillWidth]}>{children}</ExpoRow>
      </ExpoColumn>
    );
  }

  return (
    <ExpoRow spacing={8} alignment={'center'} testID={testID}>
      <ExpoIcon name={Icon.select(icon)} size={24} color={theme.colors.primary} style={styles.iconCircle} />
      {label && <ExpoText>{label}</ExpoText>}
      <ExpoRow alignment={'end'} modifiers={[AppModifiers.fillWidth]}>
        <ExpoSpacer />
        {children}
      </ExpoRow>
    </ExpoRow>
  );
};
