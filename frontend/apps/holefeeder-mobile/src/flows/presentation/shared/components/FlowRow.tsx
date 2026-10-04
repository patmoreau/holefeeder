import { DateOnly, LocalFormatter, Money, today } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { TagList } from '@/flows/core/flows/tag-list';
import { AppChip } from '@/shared/presentation/components/native/AppChip';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppRow } from '@/shared/presentation/components/native/AppRow';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';

export const FlowRowIcon = ({ color }: { color: string }) => <AppIcon name={AppIconMap.category.ios} size={20} color={color} />;

export const FlowRowAmount = ({ amount, date }: { amount: Money; date: DateOnly }) => {
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();

  return (
    <AppColumn alignment={'end'}>
      <AppText variant={'default'}>{LocalFormatter.currency(amount, currentLocale, currencyCode)}</AppText>
      <AppText variant={'footnote'}>{LocalFormatter.date(date, today(), currentLocale, t)}</AppText>
    </AppColumn>
  );
};

export const FlowRowDetails = ({ categoryName, showsCategory, tags }: { categoryName: string; showsCategory: boolean; tags: TagList }) => (
  <AppRow spacing={4} alignment={'center'}>
    {showsCategory && <AppText variant={'footnote'}>{categoryName}</AppText>}
    {tags.map((tag) => (
      <AppChip key={tag} selected={true} label={tag} />
    ))}
  </AppRow>
);
