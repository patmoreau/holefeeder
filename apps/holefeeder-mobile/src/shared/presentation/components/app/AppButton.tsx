import { Icon } from '@expo/ui';
import { buttonStyle, ModifierConfig, tint } from '@expo/ui/swift-ui/modifiers';
import React from 'react';
import { Platform, StyleProp, ViewStyle } from 'react-native';
import { AppButtonVariant } from '@/shared/presentation/components/AppButtonVariant';
import { ExpoButton } from '@/shared/presentation/components/expo/ExpoButton';
import { ExpoIcon } from '@/shared/presentation/components/expo/ExpoIcon';
import { ExpoRow } from '@/shared/presentation/components/expo/ExpoRow';
import { ExpoText } from '@/shared/presentation/components/expo/ExpoText';
import { AppIconsMappings } from '@/shared/presentation/core/app-icons';
import { useTheme } from '@/shared/theme/core/use-theme';
import { Theme } from '@/types/theme/theme';

export type ButtonProps = {
  label?: string;
  icon?: AppIconsMappings;
  iconPosition?: 'left' | 'right';
  variant?: AppButtonVariant;
  onPress?: () => void;
  children?: React.ReactNode;
  style?: StyleProp<ViewStyle>;
  color?: string;
  disabled?: boolean;
};

const variantMapping: Record<
  AppButtonVariant,
  'automatic' | 'bordered' | 'borderedProminent' | 'borderless' | 'glass' | 'glassProminent' | 'plain'
> = {
  primary: 'glassProminent',
  secondary: 'glass',
  destructive: 'glassProminent',
  link: 'plain',
  mini: 'bordered',
};

const variantColor = (variant: AppButtonVariant, theme: Theme) => {
  switch (variant) {
    case 'primary':
      return theme.colors.primary;
    case 'secondary':
      return theme.colors.secondary;
    case 'destructive':
      return theme.colors.destructive;
    case 'link':
      return theme.colors.link;
    default:
      return theme.colors.secondary;
  }
};

const variantIconColor = (variant: AppButtonVariant, theme: Theme) => {
  switch (variant) {
    case 'primary':
      return theme.colors.primaryText;
    case 'secondary':
      return theme.colors.secondaryText;
    case 'destructive':
      return theme.colors.primaryText;
    case 'link':
      return theme.colors.link;
    default:
      return theme.colors.secondary;
  }
};

export const AppButton = ({
  label,
  icon,
  iconPosition = 'left',
  variant = AppButtonVariant.secondary,
  onPress = () => {},
  style,
  disabled,
}: ButtonProps) => {
  const { theme } = useTheme();

  const modifiers: ModifierConfig[] = [];
  if (Platform.OS === 'ios') {
    modifiers.push(buttonStyle(variantMapping[variant]));
    modifiers.push(tint(variantColor(variant, theme)));
  }
  if (!icon) {
    return <ExpoButton label={label} onPress={onPress} modifiers={modifiers} disabled={disabled} />;
  }

  const buttonIcon = <ExpoIcon name={Icon.select(icon)} size={20} color={variantIconColor(variant, theme)} />;

  return (
    <ExpoButton label={label} onPress={onPress} modifiers={modifiers} disabled={disabled}>
      <ExpoRow spacing={2}>
        {icon && iconPosition === 'left' && buttonIcon}
        {label && <ExpoText>{label}</ExpoText>}
        {icon && iconPosition === 'right' && buttonIcon}
      </ExpoRow>
    </ExpoButton>
  );
};
