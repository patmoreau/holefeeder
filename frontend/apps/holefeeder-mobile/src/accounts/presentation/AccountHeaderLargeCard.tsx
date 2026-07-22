import { LocalFormatter, today, Variation } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { AccountDetail } from '@/accounts/core/account-detail';
import { AccountType } from '@/accounts/core/account-type';
import { tk } from '@/i18n/translations';
import { AppText } from '@/shared/presentation/components/AppText';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useStyles } from '@/shared/theme/core/use-styles';
import { borderRadius, fontSize, fontWeight, spacing } from '@/types/theme/design-tokens';
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
  pill: {
    borderRadius: borderRadius.full,
    paddingHorizontal: spacing.sm,
    paddingVertical: spacing.xs,
  },
  positivePill: {
    backgroundColor: theme.colors.positiveBackground,
  },
  negativePill: {
    backgroundColor: theme.colors.negativeBackground,
  },
  positiveText: {
    color: theme.colors.positive,
    fontWeight: fontWeight.semiBold,
  },
  negativeText: {
    color: theme.colors.negative,
    fontWeight: fontWeight.semiBold,
  },
});

export const AccountHeaderLargeCard = ({ account }: { account: AccountDetail }) => {
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const styles = useStyles(createStyles);

  const isPositive = Variation.multiply(account.balance, AccountType.multiplier[account.type]) >= 0;

  return (
    <>
      <AppText variant={'title'} style={styles.textColor}>
        {account.name}
      </AppText>
      <AppText variant={'largeTitle'} style={styles.largeTitle}>
        {LocalFormatter.currency(account.balance, currentLocale, currencyCode)}
      </AppText>
      <View style={styles.divider} />
      <View style={{ flexDirection: 'row', justifyContent: 'space-between' }}>
        <View style={{ flex: 1, alignItems: 'center' }}>
          <AppText variant={'subtitle'} style={styles.subtitle}>
            {t(tk.accountCard.updated)}
          </AppText>
          <View style={[styles.pill, styles.positivePill]}>
            <AppText style={styles.positiveText} adjustsFontSizeToFit>
              {LocalFormatter.date(account.lastTransactionDate!, today(), currentLocale, t)}
            </AppText>
          </View>
        </View>
        <View style={{ flex: 1, alignItems: 'center' }}>
          <AppText variant={'subtitle'} style={styles.subtitle}>
            {t(tk.accountCard.projected)}
          </AppText>
          <View style={[styles.pill, isPositive ? styles.positivePill : styles.negativePill]}>
            <AppText style={isPositive ? styles.positiveText : styles.negativeText} adjustsFontSizeToFit>
              {LocalFormatter.currency(account.projectedBalance, currentLocale, currencyCode)}
            </AppText>
          </View>
          {account.upcomingVariation !== 0 && (
            <AppText variant={'footnote'} style={[isPositive ? styles.positiveText : styles.negativeText, { opacity: 0.7 }]}>
              {account.upcomingVariation >= 0 ? '+' : ''}
              {LocalFormatter.currency(account.upcomingVariation, currentLocale, currencyCode)}
            </AppText>
          )}
        </View>
      </View>
    </>
  );
};
