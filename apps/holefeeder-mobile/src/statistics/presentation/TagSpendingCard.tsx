import { LocalFormatter } from '@holefeeder/shared/core';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppListItem } from '@/shared/presentation/components/native/AppListItem';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { TagSpending } from '@/statistics/core/tag-spending';

type TagSpendingCardProps = {
  item: TagSpending;
};

export const TagSpendingCard = ({ item }: TagSpendingCardProps) => {
  const { currentLocale, currencyCode } = useLocaleFormatter();

  return (
    <AppListItem>
      <AppListItem.Leading>
        <AppIcon name={AppIconMap.tag.ios} size={16} />
      </AppListItem.Leading>
      <AppText variant="default">{item.tag}</AppText>
      <AppListItem.Trailing>
        <AppText variant="defaultSemiBold">{LocalFormatter.currency(item.spentAmount, currentLocale, currencyCode)}</AppText>
      </AppListItem.Trailing>
    </AppListItem>
  );
};
