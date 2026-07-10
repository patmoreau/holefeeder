import { LocalFormatter, Money } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
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
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();

  const hasAverage = Money.toCents(item.avgAmount) > 0;

  return (
    <AppListItem>
      <AppListItem.Leading>
        <AppIcon name={AppIconMap.tag.ios} size={16} />
      </AppListItem.Leading>
      <AppText variant="default">{item.tag}</AppText>
      <AppListItem.Trailing>
        <AppColumn alignment="end">
          <AppText variant="defaultSemiBold">{LocalFormatter.currency(item.spentAmount, currentLocale, currencyCode)}</AppText>
          {hasAverage && (
            <AppText variant="footnote">
              {t(tk.insights.tagBreakdown.avgPerPeriod, { amount: LocalFormatter.currency(item.avgAmount, currentLocale, currencyCode) })}
            </AppText>
          )}
        </AppColumn>
      </AppListItem.Trailing>
    </AppListItem>
  );
};
