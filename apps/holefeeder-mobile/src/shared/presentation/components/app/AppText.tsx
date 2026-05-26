import { UniversalTextStyle } from '@expo/ui';
import { useStyles } from '@/shared/theme/core/use-styles';
import { fontWeight, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';
import { ExpoText, ExpoTextProps } from '../expo/ExpoText';

export type ThemedTextProps = ExpoTextProps & {
  variant?: 'default' | 'defaultSemiBold' | 'display' | 'errorField' | 'footnote' | 'largeTitle' | 'link' | 'subtitle' | 'title';
  adjustsFontSizeToFit?: boolean;
};

const createStyles = (theme: Theme) => ({
  default: {
    ...theme.typography.body,
    color: theme.colors.text,
  },
  display: {
    ...theme.typography.display,
    color: theme.colors.text,
  },
  largeTitle: {
    ...theme.typography.largeTitle,
    color: theme.colors.text,
  },
  title: {
    ...theme.typography.title,
    color: theme.colors.text,
  },
  defaultSemiBold: {
    ...theme.typography.body,
    fontWeight: fontWeight.semiBold,
    color: theme.colors.text,
  },
  subtitle: {
    ...theme.typography.subtitle,
    color: theme.colors.text + '60',
  },
  footnote: {
    ...theme.typography.footnote,
    color: theme.colors.text + '60',
  },
  link: {
    color: theme.colors.link,
  },
  errorField: {
    ...theme.typography.errorField,
    color: theme.colors.error,
  },
});

const errorFieldWrapperStyle = {
  marginTop: spacing.xs,
  marginLeft: 36 + spacing.sm,
  marginBottom: spacing.sm,
};

export const AppText = ({ textStyle, variant = 'default', adjustsFontSizeToFit, ...props }: ThemedTextProps) => {
  const styles = useStyles(createStyles);
  const variantStyle = styles[variant] as UniversalTextStyle;

  return <ExpoText textStyle={{ ...variantStyle, ...textStyle }} numberOfLines={adjustsFontSizeToFit ? 1 : undefined} {...props} />;
};
