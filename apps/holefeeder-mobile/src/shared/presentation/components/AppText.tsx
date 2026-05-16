import { Text, type TextProps } from 'react-native';
import { useStyles } from '@/shared/theme/core/use-styles';
import { fontWeight } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

export type ThemedTextProps = TextProps & {
  variant?: 'default' | 'defaultSemiBold' | 'errorField' | 'footnote' | 'largeTitle' | 'link' | 'subtitle' | 'title';
  adjustsFontSizeToFit?: boolean;
};

const createStyles = (theme: Theme) => ({
  default: {
    ...theme.typography.body,
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

export const AppText = ({ style, variant = 'default', adjustsFontSizeToFit, ...props }: ThemedTextProps) => {
  const styles = useStyles(createStyles);

  return (
    <Text
      style={[
        variant === 'default' && styles.default,
        variant === 'defaultSemiBold' && styles.defaultSemiBold,
        variant === 'errorField' && styles.errorField,
        variant === 'footnote' && styles.footnote,
        variant === 'largeTitle' && styles.largeTitle,
        variant === 'link' && styles.link,
        variant === 'subtitle' && styles.subtitle,
        variant === 'title' && styles.title,
        style,
      ]}
      adjustsFontSizeToFit={adjustsFontSizeToFit}
      minimumFontScale={adjustsFontSizeToFit ? 0.5 : undefined}
      numberOfLines={adjustsFontSizeToFit ? 1 : undefined}
      {...props}
    />
  );
};
