import { Icon } from '@expo/ui';
import React from 'react';
import { ExpoIcon } from '@/shared/presentation/components/native/expo/ExpoIcon';
import { ExpoRow } from '@/shared/presentation/components/native/expo/ExpoRow';
import { ExpoSpacer } from '@/shared/presentation/components/native/expo/ExpoSpacer';
import { ExpoText } from '@/shared/presentation/components/native/expo/ExpoText';
import { AppIconMap, UniversalIcon } from '@/shared/presentation/core/app-icon-map';
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
  onPress?: (() => void) | undefined;
  error?: string;
  testID?: string;
};

export const AppFieldLink = ({ label, icon, onPress, testID }: FieldProps) => {
  const styles = useStyles(createStyles);
  const { theme } = useTheme();

  return (
    <ExpoRow spacing={8} alignment={'center'} testID={testID} onPress={onPress}>
      <ExpoIcon name={Icon.select(icon)} size={24} color={theme.colors.primary} style={styles.iconCircle} />
      {label && <ExpoText>{label}</ExpoText>}
      <ExpoSpacer />
      <ExpoIcon name={Icon.select(AppIconMap.expand)} size={16} color={theme.colors.link} />
    </ExpoRow>
  );
};
