import React, { useRef } from 'react';
import { Pressable, View, type ViewProps } from 'react-native';
import type { CardLayout } from '@/dashboard/presentation/components/AccountCard';
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

export type TransactionCardProps = ViewProps & {
  transaction: Transaction;
  onPress?: (id: Id, layout: CardLayout) => void;
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

export const TransactionCard = ({ transaction, onPress, ...props }: TransactionCardProps) => {
  const { formatCurrency, formatDate } = useLocaleFormatter();
  const styles = useStyles(createStyles);
  const pressableRef = useRef<View>(null);

  const amountStyle = transaction.categoryType === CategoryTypes.gain ? styles.positiveAmount : styles.negativeAmount;

  const handlePress = () => {
    if (!onPress) return;
    pressableRef.current?.measureInWindow((x, y, w, h) => {
      onPress(transaction.id, { x, y, width: w, height: h });
    });
  };

  return (
    <Pressable ref={pressableRef} onPress={handlePress}>
      <AppCard {...props}>
        <View style={styles.cardDescription}>
          <AppText variant={'defaultSemiBold'} adjustsFontSizeToFit>
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
          <AppText variant={'default'} adjustsFontSizeToFit style={amountStyle}>
            {formatCurrency(transaction.amount)}
          </AppText>
          <AppText variant={'footnote'}>{formatDate(transaction.date, today())}</AppText>
        </View>
      </AppCard>
    </Pressable>
  );
};
