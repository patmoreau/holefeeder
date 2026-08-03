import { DateIntervalType, DateIntervalTypes, LocalFormatter } from '@holefeeder/shared/core';
import { router } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { Cashflow } from '@/flows/core/flows/cashflow';
import { DeactivateUpcomingFlowUseCase } from '@/flows/core/flows/deactivate-upcoming/deactivate-upcoming-flow-use-case';
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

const tkIntervalTypes: Record<DateIntervalType, string> = {
  [DateIntervalTypes.daily]: tk.dateIntervalTypePicker.daily,
  [DateIntervalTypes.weekly]: tk.dateIntervalTypePicker.weekly,
  [DateIntervalTypes.monthly]: tk.dateIntervalTypePicker.monthly,
  [DateIntervalTypes.yearly]: tk.dateIntervalTypePicker.yearly,
  [DateIntervalTypes.oneTime]: tk.dateIntervalTypePicker.oneTime,
};

export type CashflowCardProps = {
  cashflow: Cashflow;
  categoryName: string;
  color?: string;
};

export const CashflowCard = ({ cashflow, categoryName, color }: CashflowCardProps) => {
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const { flowRepository } = useRepositories();
  const { showDeleteAlert } = showAlert(t);

  const title = cashflow.description.length > 0 ? cashflow.description : categoryName;

  const intervalLabel = t(tkIntervalTypes[cashflow.intervalType]);
  const cadence = cashflow.frequency > 1 ? `${cashflow.frequency} × ${intervalLabel}` : intervalLabel;

  const handleDelete = () => {
    showDeleteAlert(title, {
      onConfirm: () => {
        DeactivateUpcomingFlowUseCase(flowRepository).execute(cashflow.id);
      },
      onCancel: () => {},
    });
  };

  return (
    <AppListItem
      onPress={() =>
        router.push({
          pathname: '/(app)/EditCashflow',
          params: { id: cashflow.id as string },
        })
      }
    >
      <AppListItem.Leading>
        <AppIcon name={AppIconMap.cashflow.ios} size={20} color={color} />
      </AppListItem.Leading>
      <AppListItem.Trailing>
        <AppColumn alignment={'end'}>
          <AppText variant={'default'}>{LocalFormatter.currency(cashflow.amount, currentLocale, currencyCode)}</AppText>
          <AppText variant={'footnote'}>{cadence}</AppText>
        </AppColumn>
      </AppListItem.Trailing>
      <AppSwipeActions>
        <AppText variant={'defaultSemiBold'} numberOfLines={1}>
          {title}
        </AppText>
        <AppSwipeActions.Actions edge="trailing" allowsFullSwipe={false}>
          <AppButton variant="destructive" label={t(tk.swipeableActions.delete)} icon={AppIconMap.delete} onPress={handleDelete} />
        </AppSwipeActions.Actions>
      </AppSwipeActions>
      <AppListItem.Supporting>
        <AppRow spacing={2}>
          {cashflow.tags.map((tag) => (
            <AppChip key={tag} selected={true} label={tag} />
          ))}
        </AppRow>
      </AppListItem.Supporting>
    </AppListItem>
  );
};
