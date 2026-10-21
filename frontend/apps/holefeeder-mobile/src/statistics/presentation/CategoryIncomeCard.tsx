import { LocalFormatter, Money } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppListItem } from '@/shared/presentation/components/native/AppListItem';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { CategoryIncome } from '@/statistics/core/category-income';

export const CategoryIncomeCard = ({ item }: { item: CategoryIncome }) => {
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();

  return (
    <AppListItem>
      <AppListItem.Leading>
        <AppIcon name={AppIconMap.category.ios} size={20} color={item.color} />
      </AppListItem.Leading>
      <AppText variant="defaultSemiBold">{item.categoryName}</AppText>
      <AppListItem.Trailing>
        <AppColumn alignment="end">
          <AppText variant="defaultSemiBold">{LocalFormatter.currency(item.receivedAmount, currentLocale, currencyCode)}</AppText>
          {Money.toCents(item.avgAmount) > 0 && (
            <AppText variant="footnote">
              {t(tk.insights.income.categoryBreakdown.avgPerPeriod, {
                amount: LocalFormatter.currency(item.avgAmount, currentLocale, currencyCode),
              })}
            </AppText>
          )}
        </AppColumn>
      </AppListItem.Trailing>
    </AppListItem>
  );
};
