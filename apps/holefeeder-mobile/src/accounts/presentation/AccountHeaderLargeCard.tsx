import { today, Variation } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { AccountDetail } from '@/accounts/core/account-detail';
import { AccountType } from '@/accounts/core/account-type';
import { tk } from '@/i18n/translations';
import { AppText } from '@/shared/presentation/components/AppText';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useStyles } from '@/shared/theme/core/use-styles';
import { fontSize, fontWeight, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  textColor: {
    color: theme.colors.primaryText,
  },
  largeTitle: {
    fontSize: fontSize!['3xl'],
    fontWeight: fontWeight.bold,
    color: theme.colors.primaryText,
    marginBottom: spacing.xs,
  },
  divider: {
    height: 1,
    backgroundColor: theme.colors.primaryText,
    opacity: 0.2,
    marginVertical: spacing.lg,
  },
  subtitle: {
    color: theme.colors.primaryText,
    opacity: 0.5,
    marginBottom: spacing.xs,
  },
  positiveAmount: {
    color: theme.colors.primaryText,
  },
  negativeAmount: {
    color: theme.colors.secondaryText,
  },
});

export const AccountHeaderLargeCard = ({ account }: { account: AccountDetail }) => {
  const { t } = useTranslation();
  const { formatCurrency, formatDate } = useLocaleFormatter();
  const styles = useStyles(createStyles);

  const isPositive = Variation.multiply(account.balance, AccountType.multiplier[account.type]) >= 0;

  return (
    <>
      <AppText variant={'title'} style={styles.textColor}>
        {account.name}
      </AppText>
      <AppText variant={'largeTitle'} style={styles.largeTitle}>
        {formatCurrency(account.balance)}
      </AppText>
      <View style={styles.divider} />
      <View style={{ flexDirection: 'row', flex: 1, justifyContent: 'space-between' }}>
        <View style={{ flex: 1, alignItems: 'center' }}>
          <AppText variant={'subtitle'} style={styles.subtitle}>
            {t(tk.accountCard.updated)}
          </AppText>
          <AppText style={styles.textColor} adjustsFontSizeToFit>
            {formatDate(account.lastTransactionDate!, today())}
          </AppText>
        </View>
        <View style={{ flex: 1, alignItems: 'center' }}>
          <AppText variant={'subtitle'} style={styles.subtitle}>
            {t(tk.accountCard.projected)}
          </AppText>
          <AppText style={[isPositive ? styles.positiveAmount : styles.negativeAmount]} adjustsFontSizeToFit>
            {formatCurrency(account.projectedBalance)}
          </AppText>
          {account.upcomingVariation !== 0 && (
            <AppText variant={'footnote'} style={[isPositive ? styles.positiveAmount : styles.negativeAmount, { opacity: 0.7 }]}>
              {account.upcomingVariation >= 0 ? '+' : ''}
              {formatCurrency(account.upcomingVariation)}
            </AppText>
          )}
        </View>
      </View>
    </>
  );
};
