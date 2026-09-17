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
  totals: {
    flexDirection: 'row' as const,
    alignItems: 'flex-start' as const,
  },
  total: {
    flex: 1,
    alignItems: 'center' as const,
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
  neutralPill: {
    backgroundColor: theme.colors.secondaryBackground,
  },
  positiveText: {
    color: theme.colors.positive,
    fontWeight: fontWeight.semiBold,
  },
  negativeText: {
    color: theme.colors.negative,
    fontWeight: fontWeight.semiBold,
  },
  neutralText: {
    color: theme.colors.amountNeutral,
    fontWeight: fontWeight.semiBold,
  },
});

type Tone = 'positive' | 'negative' | 'neutral';

export const AccountHeaderLargeCard = ({ account }: { account: AccountDetail }) => {
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const styles = useStyles(createStyles);

  const isPositive = Variation.multiply(account.balance, AccountType.multiplier[account.type]) >= 0;
  const upcomingChange = AccountDetail.upcomingChange(account);
  const upcomingTone: Tone = upcomingChange === 0 ? 'neutral' : AccountDetail.isUpcomingFavourable(account) ? 'positive' : 'negative';
  const upcomingSign = upcomingChange > 0 ? '+ ' : upcomingChange < 0 ? '- ' : '';

  const pillStyle = { positive: styles.positivePill, negative: styles.negativePill, neutral: styles.neutralPill };
  const textStyle = { positive: styles.positiveText, negative: styles.negativeText, neutral: styles.neutralText };

  const total = (label: string, value: string, tone: Tone) => (
    <View style={styles.total}>
      <AppText variant={'subtitle'} style={styles.subtitle} numberOfLines={1}>
        {label}
      </AppText>
      <View style={[styles.pill, pillStyle[tone]]}>
        <AppText style={textStyle[tone]} numberOfLines={1} adjustsFontSizeToFit>
          {value}
        </AppText>
      </View>
    </View>
  );

  return (
    <>
      <AppText variant={'title'} style={styles.textColor}>
        {account.name}
      </AppText>
      <AppText variant={'largeTitle'} style={styles.largeTitle}>
        {LocalFormatter.currency(account.balance, currentLocale, currencyCode)}
      </AppText>
      <View style={styles.divider} />
      <View style={styles.totals}>
        {total(t(tk.accountCard.updated), LocalFormatter.date(account.lastTransactionDate!, today(), currentLocale, t), 'positive')}
        {total(
          t(tk.accountCard.upcoming),
          `${upcomingSign}${LocalFormatter.currency(Math.abs(upcomingChange), currentLocale, currencyCode)}`,
          upcomingTone
        )}
        {total(
          t(tk.accountCard.projected),
          LocalFormatter.currency(account.projectedBalance, currentLocale, currencyCode),
          isPositive ? 'positive' : 'negative'
        )}
      </View>
    </>
  );
};
