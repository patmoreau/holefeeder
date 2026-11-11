import { Logger, Result } from '@holefeeder/shared/core';
import { SpendingSnapshot } from '@/dashboard/core/publish-spending-widget/spending-snapshot';
import { SpendingWidgetPublisher, SpendingWidgetPublisherErrors } from '@/dashboard/core/publish-spending-widget/spending-widget-publisher';
import SpendingWidget, { SpendingWidgetProps } from '@/dashboard/widgets/SpendingWidget';
import { displayAmount } from '@/shared/presentation/core/display-amount';

export type SpendingWidgetDisplay = {
  formatAmount: (amount: number) => string;
  formatTrend: (amount: string) => string;
  labels: { title: string; projected: string; openApp: string };
  logo?: string;
};

const logger = Logger.create('SpendingWidgetPublisherWithExpoWidgets');

export const SpendingWidgetPublisherWithExpoWidgets = (display: SpendingWidgetDisplay): SpendingWidgetPublisher => {
  const snapshotProps = (snapshot: SpendingSnapshot): SpendingWidgetProps => {
    const projected = displayAmount(snapshot.projected);
    return {
      state: 'ready',
      logo: display.logo,
      title: display.labels.title,
      spending: display.formatAmount(snapshot.spending),
      trend: snapshot.trend && {
        text: display.formatTrend(display.formatAmount(snapshot.trend.amount)),
        direction: snapshot.trend.isOver ? 'up' : 'down',
      },
      projected: { label: display.labels.projected, text: display.formatAmount(projected.amount), tone: projected.tone },
    };
  };

  const update = (apply: () => void): Result<void> => {
    try {
      apply();
      return Result.success();
    } catch (error) {
      if (error instanceof Error) {
        logger.error(`${SpendingWidgetPublisherErrors.publishFailed}: `, error.message);
      }
      return Result.failure([SpendingWidgetPublisherErrors.publishFailed]);
    }
  };

  const publish = async (snapshot: SpendingSnapshot): Promise<Result<void>> =>
    update(() => SpendingWidget.updateSnapshot(snapshotProps(snapshot)));

  const clear = async (): Promise<Result<void>> =>
    update(() => SpendingWidget.updateSnapshot({ state: 'signed-out', logo: display.logo, message: display.labels.openApp }));

  return {
    publish: publish,
    clear: clear,
  };
};
