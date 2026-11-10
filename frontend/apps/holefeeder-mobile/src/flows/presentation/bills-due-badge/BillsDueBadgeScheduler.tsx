import { BadgeSchedulerWithExpoNotifications } from '@/flows/notifications/badge-scheduler-with-expo-notifications';
import { useScheduleBillsDueBadge } from '@/flows/presentation/bills-due-badge/use-schedule-bills-due-badge';

const scheduler = BadgeSchedulerWithExpoNotifications();

export const BillsDueBadgeScheduler = () => {
  useScheduleBillsDueBadge(scheduler);
  return null;
};
