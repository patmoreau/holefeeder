import { Button, HStack, Image } from '@expo/ui/swift-ui';
import { buttonStyle, fixedSize, frame, tint } from '@expo/ui/swift-ui/modifiers';
import React from 'react';
import { StyleProp, StyleSheet, ViewStyle } from 'react-native';
import { AppButtonVariant } from '@/shared/presentation/components/AppButtonVariant';
import { AppHost } from '@/shared/presentation/components/AppHost.ios';
import { AppIcons } from '@/shared/presentation/icons';
import { useTheme } from '@/shared/theme/core/use-theme';
import { spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

export type ButtonProps = {
  label?: string;
  icon?: AppIcons;
  iconPosition?: 'left' | 'right';
  variant?: AppButtonVariant;
  onPress?: () => void;
  children?: React.ReactNode;
  style?: StyleProp<ViewStyle>;
  color?: string;
  disabled?: boolean;
  testID?: string;
};

const variantMapping: Record<
  AppButtonVariant,
  'automatic' | 'bordered' | 'borderedProminent' | 'borderless' | 'glass' | 'glassProminent' | 'plain'
> = {
  primary: 'glassProminent',
  secondary: 'glass',
  destructive: 'glassProminent',
  link: 'plain',
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

export const AppButton = ({
  label,
  icon,
  iconPosition = 'left',
  variant = AppButtonVariant.secondary,
  onPress = () => {},
  style,
  disabled,
  testID,
}: ButtonProps) => {
  const { theme } = useTheme();
  const handlePress = () => {
    if (!disabled) {
      onPress();
    }
  };

  const flatStyle = StyleSheet.flatten(style);
  const width = typeof flatStyle?.width === 'number' ? flatStyle.width : undefined;
  const height = typeof flatStyle?.height === 'number' ? flatStyle.height : undefined;

  if (!label && icon)
    return (
      <AppHost matchContents={false} style={style} testID={testID}>
        <Image systemName={icon} onPress={onPress} />
      </AppHost>
    );

  const modifiers = [];
  if (width !== undefined || height !== undefined) {
    modifiers.push(frame({ width, height, alignment: 'center' }));
  } else {
    modifiers.push(fixedSize({ horizontal: true, vertical: true }));
    modifiers.push(frame({ alignment: 'center' }));
  }
  modifiers.push(buttonStyle(variantMapping[variant]));
  modifiers.push(tint(variantMapping[variant]));

  const buttonIcon = iconPosition === 'left' ? icon : undefined;

  return (
    <AppHost style={{ margin: spacing.sm }} testID={testID}>
      <HStack>
        <Button
          label={label}
          systemImage={buttonIcon}
          role={variant === AppButtonVariant.destructive ? 'destructive' : undefined}
          onPress={handlePress}
          modifiers={modifiers}
        />
        {icon && iconPosition === 'right' && <Image systemName={icon} color={variantColor(variant, theme)} />}
      </HStack>
    </AppHost>
  );
};
