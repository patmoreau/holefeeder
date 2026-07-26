import { LocalFormatter, today } from '@holefeeder/shared/core';
import { router } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { useUpcomingFlow } from '@/dashboard/presentation/core/use-pay-form';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { tk } from '@/i18n/translations';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppChip } from '@/shared/presentation/components/native/AppChip';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppListItem } from '@/shared/presentation/components/native/AppListItem';
import { AppRow } from '@/shared/presentation/components/native/AppRow';
import { AppSwipeActions } from '@/shared/presentation/components/native/AppSwipeActions';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { showAlert } from '@/shared/presentation/core/show-alert';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export type UpcomingCardProps = {
  upcomingFlow: UpcomingFlow;
};

export const UpcomingCard = ({ upcomingFlow }: UpcomingCardProps) => {
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const repositories = useRepositories();
  const upcomingFlowUseCase = useUpcomingFlow(repositories);
  const { showDeleteAlert } = showAlert(t);

  const handlePay = () => upcomingFlowUseCase.pay(upcomingFlow);

  const handleClear = () => upcomingFlowUseCase.clear(upcomingFlow);

  const handleDelete = () => {
    showDeleteAlert(upcomingFlow.description, {
      onConfirm: () => {
        upcomingFlowUseCase.delete(upcomingFlow);
      },
      onCancel: () => {},
    });
  };

  return (
    <AppListItem
      onPress={() =>
        router.push({
          pathname: '/(app)/PayUpcoming',
          params: { data: JSON.stringify(upcomingFlow) },
        })
      }
    >
      <AppListItem.Leading>
        <AppIcon name={AppIconMap.purchase.ios} size={20} color="#FFD60A" />
      </AppListItem.Leading>
      <AppListItem.Trailing>
        <AppColumn alignment={'end'}>
          <AppText variant={'default'}>{LocalFormatter.currency(upcomingFlow.amount, currentLocale, currencyCode)}</AppText>
          <AppText variant={'footnote'}>{LocalFormatter.date(upcomingFlow.date, today(), currentLocale, t)}</AppText>
        </AppColumn>
      </AppListItem.Trailing>
      <AppSwipeActions>
        <AppText variant={'defaultSemiBold'} numberOfLines={1}>
          {upcomingFlow.description}
        </AppText>
        <AppSwipeActions.Actions edge="leading" allowsFullSwipe={true}>
          <AppButton variant="primary" label={t(tk.swipeableActions.pay)} icon={AppIconMap.purchase} onPress={handlePay} />
        </AppSwipeActions.Actions>
        <AppSwipeActions.Actions edge="trailing" allowsFullSwipe={false}>
          <AppButton variant="destructive" label={t(tk.swipeableActions.delete)} icon={AppIconMap.delete} onPress={handleDelete} />
          <AppButton variant="secondary" label={t(tk.swipeableActions.clear)} icon={AppIconMap.cancel} onPress={handleClear} />
        </AppSwipeActions.Actions>
      </AppSwipeActions>
      <AppListItem.Supporting>
        <AppRow spacing={2}>
          {upcomingFlow.tags.map((tag) => (
            <AppChip key={tag} selected={true} label={tag} />
          ))}
        </AppRow>
      </AppListItem.Supporting>
    </AppListItem>
  );
};
