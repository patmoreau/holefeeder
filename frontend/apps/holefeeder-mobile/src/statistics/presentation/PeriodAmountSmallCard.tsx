import { LocalFormatter } from '@holefeeder/shared/core';
import { View } from 'react-native';
import { AppText } from '@/shared/presentation/components/AppText';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useStyles } from '@/shared/theme/core/use-styles';
import { fontWeight, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  row: {
    flexDirection: 'row' as const,
    alignItems: 'center' as const,
    gap: spacing.sm,
    flexWrap: 'wrap' as const,
  },
  label: {
    color: theme.colors.primaryText,
  },
  amount: {
    color: theme.colors.primaryText,
    fontWeight: fontWeight.semiBold,
  },
});

export const PeriodAmountSmallCard = ({ label, amount, testID }: { label: string; amount: number; testID?: string }) => {
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const styles = useStyles(createStyles);

  return (
    <View style={styles.row} testID={testID}>
      <AppText variant={'subtitle'} style={styles.label}>
        {label}
      </AppText>
      <AppText variant={'title'} style={styles.amount}>
        {LocalFormatter.currency(amount, currentLocale, currencyCode)}
      </AppText>
    </View>
  );
};
