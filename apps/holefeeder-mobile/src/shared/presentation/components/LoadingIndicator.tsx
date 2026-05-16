import { useTranslation } from 'react-i18next';
import { ActivityIndicator, ActivityIndicatorProps } from 'react-native';
import { tk } from '@/i18n/translations';
import { useTheme } from '@/shared/theme/core/use-theme';

type LoadingIndicatorProps = Omit<ActivityIndicatorProps, 'size' | 'color'> & {
  size?: 'small' | 'large';
  variant?: 'primary' | 'secondary';
  withBackground?: boolean;
};

export const LoadingIndicator = ({
  size = 'large',
  variant = 'primary',
  withBackground = true,
  accessibilityRole = 'progressbar',
  ...props
}: LoadingIndicatorProps) => {
  const { t } = useTranslation();
  const { theme } = useTheme();

  const variantColor = variant === 'primary' ? theme.colors.primary : theme.colors.secondary;

  return (
    <ActivityIndicator
      accessibilityLabel={t(tk.common.loading)}
      accessibilityRole={accessibilityRole}
      size={size}
      color={variantColor}
      {...props}
    />
  );
};
