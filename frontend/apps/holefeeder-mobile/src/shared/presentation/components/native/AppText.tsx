import { UniversalTextStyle } from '@expo/ui';
import { font } from '@expo/ui/swift-ui/modifiers';
import { Platform } from 'react-native';
import { useStyles } from '@/shared/theme/core/use-styles';
import { fontWeight } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';
import { ExpoText, ExpoTextProps } from './expo/ExpoText';

export type ThemedTextVariant =
  | 'default'
  | 'defaultSemiBold'
  | 'display'
  | 'errorField'
  | 'footnote'
  | 'largeTitle'
  | 'link'
  | 'subtitle'
  | 'title';

export type ThemedTextProps = ExpoTextProps & {
  variant?: ThemedTextVariant;
  adjustsFontSizeToFit?: boolean;
};

type FontParams = Parameters<typeof font>[0];
type FontTextStyle = NonNullable<FontParams['textStyle']>;
type FontWeight = NonNullable<FontParams['weight']>;

const iosTextStyles: Record<ThemedTextVariant, FontTextStyle | undefined> = {
  default: 'body',
  defaultSemiBold: 'body',
  display: undefined,
  errorField: 'footnote',
  footnote: 'footnote',
  largeTitle: 'largeTitle',
  link: 'body',
  subtitle: 'subheadline',
  title: 'title3',
};

const iosFontWeights: Partial<Record<ThemedTextVariant, FontWeight>> = {
  defaultSemiBold: 'semibold',
  largeTitle: 'bold',
  title: 'semibold',
};

export const dynamicTypeFontModifier = (variant: ThemedTextVariant) => {
  const textStyle = iosTextStyles[variant];

  return textStyle ? font({ textStyle, weight: iosFontWeights[variant] }) : undefined;
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

export const AppText = ({ textStyle, variant = 'default', adjustsFontSizeToFit, modifiers = [], ...props }: ThemedTextProps) => {
  const styles = useStyles(createStyles);
  const variantStyle = styles[variant] as UniversalTextStyle;
  const dynamicTypeFont = Platform.OS === 'ios' ? dynamicTypeFontModifier(variant) : undefined;

  return (
    <ExpoText
      textStyle={{ ...variantStyle, ...textStyle }}
      numberOfLines={adjustsFontSizeToFit ? 1 : undefined}
      modifiers={dynamicTypeFont ? [dynamicTypeFont, ...modifiers] : modifiers}
      {...props}
    />
  );
};
