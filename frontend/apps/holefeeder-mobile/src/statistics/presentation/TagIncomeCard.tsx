import { LocalFormatter, Money } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppListItem } from '@/shared/presentation/components/native/AppListItem';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { TagIncome } from '@/statistics/core/tag-income';

type TagIncomeCardProps = {
  item: TagIncome;
};

export const TagIncomeCard = ({ item }: TagIncomeCardProps) => {
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();

  const hasAverage = Money.toCents(item.avgAmount) > 0;

  return (
    <AppListItem>
      <AppListItem.Leading>
        <AppIcon name={AppIconMap.tag.ios} size={20} />
      </AppListItem.Leading>
      <AppText variant="defaultSemiBold">{item.tag}</AppText>
      <AppListItem.Trailing>
        <AppColumn alignment="end">
          <AppText variant="defaultSemiBold">{LocalFormatter.currency(item.receivedAmount, currentLocale, currencyCode)}</AppText>
          {hasAverage && (
            <AppText variant="footnote">
              {t(tk.insights.income.tagBreakdown.avgPerPeriod, {
                amount: LocalFormatter.currency(item.avgAmount, currentLocale, currencyCode),
              })}
            </AppText>
          )}
        </AppColumn>
      </AppListItem.Trailing>
    </AppListItem>
  );
};
