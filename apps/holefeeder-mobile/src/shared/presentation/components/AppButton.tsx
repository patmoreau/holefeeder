import React from 'react';
import { Pressable, PressableProps, StyleProp, View, ViewStyle } from 'react-native';
import { AppButtonVariant } from '@/shared/presentation/components/AppButtonVariant';
import { AppText } from '@/shared/presentation/components/AppText';
import { IconSymbol } from '@/shared/presentation/components/IconSymbol';
import { AppIcons } from '@/shared/presentation/icons';
import { useStyles } from '@/shared/theme/core/use-styles';
import { Theme } from '@/types/theme/theme';

export type ButtonProps = Omit<PressableProps, 'onPress'> & {
  label?: string;
  icon?: AppIcons;
  iconPosition?: 'left' | 'right';
  variant?: AppButtonVariant;
  onPress?: () => void;
  style?: StyleProp<ViewStyle>;
  color?: string;
};

const createStyles = (theme: Theme) => ({
  primaryButton: {
    ...theme.styles.buttons.primary,
    color: theme.colors.primaryText,
    backgroundColor: theme.colors.primary,
  },
  destructiveButton: {
    ...theme.styles.buttons.destructive,
    color: theme.colors.primaryText,
    backgroundColor: theme.colors.destructive,
  },
  secondaryButton: {
    ...theme.styles.buttons.secondary,
    color: theme.colors.primaryText,
    backgroundColor: theme.colors.secondary,
  },
  linkButton: {
    ...theme.styles.buttons.link,
    color: theme.colors.link,
    backgroundColor: 'transparent',
  },
});

export function AppButton({
  label,
  icon,
  iconPosition = 'left',
  variant = AppButtonVariant.secondary,
  onPress = () => {},
  color,
  children,
  style,
  ...props
}: ButtonProps) {
  const styles = useStyles(createStyles);

  const getVariantStyle = () => {
    switch (variant) {
      case 'primary':
        return styles.primaryButton;
      case 'destructive':
        return styles.destructiveButton;
      case 'link':
        return styles.linkButton;
      case 'secondary':
      default:
        return styles.secondaryButton;
    }
  };

  const variantStyle = getVariantStyle();
  const buttonColor = color ?? variantStyle.color;

  return (
    <Pressable style={[variantStyle, style]} onPress={onPress} accessibilityRole="button" {...props}>
      <View style={{ flexDirection: 'row', alignItems: 'center' }}>
        {icon && iconPosition === 'left' && <IconSymbol name={icon} color={buttonColor} />}
        {label && (
          <AppText style={{ color: buttonColor }} variant="default">
            {label}
          </AppText>
        )}
        {icon && iconPosition === 'right' && <IconSymbol name={icon} color={buttonColor} />}
      </View>
    </Pressable>
  );
}
