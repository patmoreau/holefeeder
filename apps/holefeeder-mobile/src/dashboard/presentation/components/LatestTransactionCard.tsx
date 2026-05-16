import { router } from 'expo-router';
import React, { useRef } from 'react';
import { Pressable, View, type ViewProps } from 'react-native';
import { CategoryTypes } from '@/flows/core/categories/category-type';
import { Transaction } from '@/flows/core/flows/transaction';
import { Id } from '@/shared/core/id';
import { today } from '@/shared/core/with-date';
import { AppCard } from '@/shared/presentation/components/AppCard';
import { AppChip } from '@/shared/presentation/components/AppChip';
import { AppText } from '@/shared/presentation/components/AppText';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useStyles } from '@/shared/theme/core/use-styles';
import { Theme } from '@/types/theme';
import { spacing } from '@/types/theme/design-tokens';

export type LatestTransactionCardProps = ViewProps & {
  transaction: Transaction;
};

const createStyles = (theme: Theme) => ({
  cardDescription: {
    flex: 1,
    flexDirection: 'column' as const,
  },
  cardAmount: {
    flexShrink: 0,
    alignItems: 'flex-end' as const,
    justifyContent: 'center' as const,
  },
  tags: {
    flexDirection: 'row' as const,
    flexWrap: 'wrap' as const,
    marginTop: spacing.xs,
  },
  positiveAmount: {
    color: theme.colors.positive,
  },
  negativeAmount: {
    color: theme.colors.negative,
  },
});

export const LatestTransactionCard = ({ transaction, ...props }: LatestTransactionCardProps) => {
  const { formatCurrency, formatDate } = useLocaleFormatter();
  const styles = useStyles(createStyles);
  const pressableRef = useRef<View>(null);

  const amountStyle = transaction.categoryType === CategoryTypes.gain ? styles.positiveAmount : styles.negativeAmount;

  const onFlowPress = (id: Id) =>
    router.push({
      pathname: '/(app)/flows/[id]',
      params: { id: id as string },
    });

  return (
    <Pressable ref={pressableRef} onPress={() => onFlowPress(transaction.id)}>
      <AppCard {...props}>
        <View style={styles.cardDescription}>
          <AppText variant={'defaultSemiBold'} numberOfLines={1} ellipsizeMode="tail">
            {transaction.description}
          </AppText>
          {transaction.tags.length > 0 && (
            <View style={styles.tags}>
              {transaction.tags.map((tag) => (
                <AppChip key={tag} selected={true} label={tag} />
              ))}
            </View>
          )}
        </View>
        <View style={styles.cardAmount}>
          <AppText variant={'default'} style={amountStyle}>
            {formatCurrency(transaction.amount)}
          </AppText>
          <AppText variant={'footnote'}>{formatDate(transaction.date, today())}</AppText>
        </View>
      </AppCard>
    </Pressable>
  );
};
