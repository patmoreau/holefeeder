import { LocalFormatter } from '@holefeeder/shared/core';
import { View } from 'react-native';
import { AccountDetail } from '@/accounts/core/account-detail';
import { AppText } from '@/shared/presentation/components/AppText';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useStyles } from '@/shared/theme/core/use-styles';
import { spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  container: {
    flexDirection: 'row' as const,
    alignItems: 'center' as const,
    gap: spacing.sm,
  },
  text: {
    color: theme.colors.primaryText,
  },
});

export const AccountHeaderSmallCard = ({ account }: { account: AccountDetail }) => {
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const styles = useStyles(createStyles);

  return (
    <View style={styles.container}>
      <AppText style={styles.text}>{account.name}</AppText>
      <AppText style={styles.text}>{LocalFormatter.currency(account.balance, currentLocale, currencyCode)}</AppText>
    </View>
  );
};
