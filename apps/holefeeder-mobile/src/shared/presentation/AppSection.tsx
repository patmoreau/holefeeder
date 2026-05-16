import { Children } from 'react';
import { View, type ViewProps } from 'react-native';
import { AppText } from '@/shared/presentation/components/AppText';
import { useStyles } from '@/shared/theme/core/use-styles';
import { Theme } from '@/types/theme/theme';

type SectionProps = ViewProps & {
  title?: string;
};

const createStyles = (theme: Theme) => ({
  section: {
    ...theme.styles.containers.section,
    backgroundColor: theme.colors.secondaryBackground,
  },
  title: {
    ...theme.typography.secondary,
    paddingLeft: 36,
    textAlign: 'left' as const,
    color: `${theme.colors.text}3C`,
  },
  divider: {
    height: 1,
    backgroundColor: theme.colors.separator,
  },
});

export const AppSection = ({ title, style, children, ...otherProps }: SectionProps) => {
  const styles = useStyles(createStyles);

  return (
    <View style={{ flexDirection: 'column' }}>
      {title && (
        <AppText style={styles.title} variant="footnote">
          {title}
        </AppText>
      )}
      <View style={[styles.section, style]} {...otherProps}>
        {Children.map(children, (child, index) => (
          <>
            {child}
            {index < Children.count(children) - 1 && <View style={styles.divider} />}
          </>
        ))}
      </View>
    </View>
  );
};
