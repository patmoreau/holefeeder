import { useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { Pressable, StyleSheet, View, type ViewProps } from 'react-native';
import Animated, { SharedTransition } from 'react-native-reanimated';
import { AccountSummary } from '@/accounts/core/account-summary';
import { AccountType } from '@/accounts/core/account-type';
import { tk } from '@/i18n/translations';
import { Id } from '@/shared/core/id';
import { Variation } from '@/shared/core/variation';
import { today } from '@/shared/core/with-date';
import { AppCard } from '@/shared/presentation/components/AppCard';
import { AppText } from '@/shared/presentation/components/AppText';
import { LoadingIndicator } from '@/shared/presentation/components/LoadingIndicator';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useStyles } from '@/shared/theme/core/use-styles';
import { fontWeight, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';
import { useAccountVariation } from '../core/use-account-variation';

export type CardLayout = { x: number; y: number; width: number; height: number };

export type AccountCardProps = ViewProps & {
  account: AccountSummary;
  width?: number;
  onPress?: (id: Id, layout: CardLayout) => void;
};

const createStyles = (theme: Theme) =>
  StyleSheet.create({
    header: {
      marginBottom: spacing.lg,
    },
    accountName: {
      ...theme.typography.title,
      color: theme.colors.text,
      marginBottom: spacing.xs,
    },
    lastUpdated: {
      color: theme.colors.secondaryText,
    },
    balanceSection: {
      marginBottom: spacing.lg,
    },
    balanceLabel: {
      ...theme.typography.footnote,
      color: theme.colors.secondaryText,
      marginBottom: spacing.xs,
      textTransform: 'uppercase',
      letterSpacing: 0.5,
    },
    balanceAmount: {
      ...theme.typography.largeTitle,
      color: theme.colors.text,
      fontWeight: fontWeight.bold,
    },
    projectedSection: {
      flexDirection: 'row',
      paddingTop: spacing.lg,
      borderTopWidth: 1,
      borderTopColor: theme.colors.separator,
    },
    projectedLabel: {
      ...theme.typography.footnote,
      color: theme.colors.secondaryText,
      marginBottom: spacing.xs,
    },
    projectedAmount: {
      ...theme.typography.title,
      fontWeight: fontWeight.semiBold,
    },
    positiveAmount: {
      color: theme.colors.positive,
    },
    negativeAmount: {
      color: theme.colors.negative,
    },
  });

export const transition = SharedTransition.duration(500).springify().damping(20).stiffness(90);

export const AccountCard = ({ account, width = 300, style, onPress, ...props }: AccountCardProps) => {
  const { t } = useTranslation();
  const { formatCurrency, formatDate } = useLocaleFormatter();
  const styles = useStyles(createStyles);
  const pressableRef = useRef<View>(null);
  const variationResult = useAccountVariation(account.id);

  const handlePress = () => {
    if (!onPress) return;
    pressableRef.current?.measureInWindow((x, y, w, h) => {
      onPress(account.id, { x, y, width: w, height: h });
    });
  };

  return (
    <Pressable ref={pressableRef} onPress={handlePress}>
      <Animated.View sharedTransitionTag={`tag-${account.id}`} sharedTransitionStyle={transition}>
        <AppCard scrollable={'horizontal'} cardWidth={width} style={style} {...props}>
          <View style={styles.header}>
            <AppText variant={'defaultSemiBold'} adjustsFontSizeToFit>
              {account.name}
            </AppText>
          </View>

          {variationResult.isLoading && (
            <View style={styles.balanceSection}>
              <LoadingIndicator size="small" withBackground={false} />
            </View>
          )}

          {variationResult.isSuccess &&
            (() => {
              const detail = variationResult.value;
              const balanceSign = detail.balance >= 0 ? '' : '-';
              const projectedSign = detail.balance >= 0 ? '' : '-';
              return (
                <>
                  <View style={styles.balanceSection}>
                    <AppText variant={'title'} adjustsFontSizeToFit>
                      {balanceSign}
                      {formatCurrency(detail.balance)}
                    </AppText>
                  </View>

                  <View style={styles.projectedSection}>
                    <View style={{ flex: 1, flexDirection: 'column', alignItems: 'flex-start' }}>
                      <AppText variant={'footnote'}>{t(tk.accountCard.updated)}</AppText>
                      <AppText variant={'default'}>{formatDate(detail.lastTransactionDate!, today())}</AppText>
                    </View>
                    <View style={{ flex: 1, flexDirection: 'column', alignItems: 'flex-end' }}>
                      <AppText variant={'footnote'}>{t(tk.accountCard.projected)}</AppText>
                      <AppText
                        variant={'defaultSemiBold'}
                        style={[
                          styles.projectedAmount,
                          Variation.multiply(detail.balance, AccountType.multiplier[detail.type]) >= 0
                            ? styles.positiveAmount
                            : styles.negativeAmount,
                        ]}
                        adjustsFontSizeToFit
                      >
                        {projectedSign}
                        {formatCurrency(detail.projectedBalance)}
                      </AppText>
                    </View>
                  </View>
                </>
              );
            })()}
        </AppCard>
      </Animated.View>
    </Pressable>
  );
};
