import { LocalFormatter } from '@holefeeder/shared/core';
import { View } from 'react-native';
import { AccountDetail } from '@/accounts/core/account-detail';
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
  name: {
    color: theme.colors.primaryText,
  },
  balance: {
    color: theme.colors.primaryText,
    fontWeight: fontWeight.semiBold,
  },
});

export const AccountHeaderSmallCard = ({ account }: { account: AccountDetail }) => {
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const styles = useStyles(createStyles);

  return (
    <View style={styles.row} testID="account-header-small">
      <AppText variant={'subtitle'} style={styles.name} numberOfLines={1}>
        {account.name}
      </AppText>
      <AppText variant={'title'} style={styles.balance}>
        {LocalFormatter.currency(account.balance, currentLocale, currencyCode)}
      </AppText>
    </View>
  );
};
