import { LocalFormatter, today } from '@holefeeder/shared/core';
import { router } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { type ViewProps } from 'react-native';
import { Transaction } from '@/flows/core/flows/transaction';
import { AppChip } from '@/shared/presentation/components/native/AppChip';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppListItem } from '@/shared/presentation/components/native/AppListItem';
import { AppRow } from '@/shared/presentation/components/native/AppRow';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';

export type LatestTransactionCardProps = ViewProps & {
  transaction: Transaction;
};

export const LatestTransactionCard = ({ transaction, ...props }: LatestTransactionCardProps) => {
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();

  return (
    <AppListItem
      onPress={() =>
        router.push({
          pathname: '/(app)/flows/[id]',
          params: { id: transaction.id as string },
        })
      }
    >
      <AppListItem.Leading>
        <AppIcon name={AppIconMap.purchase.ios} size={20} color="#FFD60A" />
      </AppListItem.Leading>
      <AppListItem.Trailing>
        <AppColumn alignment={'end'}>
          <AppText variant={'default'}>{LocalFormatter.currency(transaction.amount, currentLocale, currencyCode)}</AppText>
          <AppText variant={'footnote'}>{LocalFormatter.date(transaction.date, today(), currentLocale, t)}</AppText>
        </AppColumn>
      </AppListItem.Trailing>
      <AppText variant={'defaultSemiBold'} numberOfLines={1}>
        {transaction.description}
      </AppText>
      <AppListItem.Supporting>
        <AppRow spacing={2}>
          {transaction.tags.map((tag) => (
            <AppChip key={tag} selected={true} label={tag} />
          ))}
        </AppRow>
      </AppListItem.Supporting>
    </AppListItem>
  );
};
