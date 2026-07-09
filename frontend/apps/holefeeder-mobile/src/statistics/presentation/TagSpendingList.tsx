import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppListForEach } from '@/shared/presentation/components/native/AppListForEach';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { useTagSpending } from './core/use-tag-spending';
import { TagSpendingCard } from './TagSpendingCard';

export const TagSpendingList = () => {
  const { t } = useTranslation();
  const result = useTagSpending();

  if (!result.isSuccess || result.value.length === 0) {
    return (
      <AppFieldSection title={t(tk.insights.tagBreakdown.title)}>
        <AppText variant="default">{t(tk.insights.tagBreakdown.empty)}</AppText>
      </AppFieldSection>
    );
  }

  return (
    <AppFieldSection title={t(tk.insights.tagBreakdown.title)}>
      <AppListForEach>
        {result.value.map((item) => (
          <TagSpendingCard key={item.tag} item={item} />
        ))}
      </AppListForEach>
    </AppFieldSection>
  );
};
