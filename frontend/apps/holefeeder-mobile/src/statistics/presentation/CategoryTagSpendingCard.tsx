import { LocalFormatter } from '@holefeeder/shared/core';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppListItem } from '@/shared/presentation/components/native/AppListItem';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { CategoryTagSpending } from '@/statistics/core/category-tag-spending';

type CategoryTagSpendingCardProps = {
  item: CategoryTagSpending;
};

export const CategoryTagSpendingCard = ({ item }: CategoryTagSpendingCardProps) => {
  const { currentLocale, currencyCode } = useLocaleFormatter();

  return (
    <AppListItem>
      <AppListItem.Leading>
        <AppIcon name={AppIconMap.tag.ios} size={16} />
      </AppListItem.Leading>
      <AppText variant="footnote">{item.tag}</AppText>
      <AppListItem.Trailing>
        <AppText variant="footnote">{LocalFormatter.currency(item.spentAmount, currentLocale, currencyCode)}</AppText>
      </AppListItem.Trailing>
    </AppListItem>
  );
};
