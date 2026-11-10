import { DateOnly, Money } from '@holefeeder/shared/core';
import { BillsDueWidgetPublisherErrors } from '@/flows/core/flows/publish-bills-due-widget/bills-due-widget-publisher';
import { BillsDueWidgetPublisherWithExpoWidgets } from '@/flows/widgets/bills-due-widget-publisher-with-expo-widgets';
import BillsDueWidget from '@/flows/widgets/BillsDueWidget';

const updateTimeline = jest.mocked(BillsDueWidget.updateTimeline);
const updateSnapshot = jest.mocked(BillsDueWidget.updateSnapshot);

const display = {
  formatAmount: (amount: Money) => `$${amount}`,
  labels: { title: 'Bills', pastDue: 'Past due', dueToday: 'Due today', openApp: 'Open the app' },
  logo: 'file:///group/widgets/widget-tile.png',
};
const total = (count: number, amount: number) => ({ count: count, amount: Money.valid(amount) });

describe('BillsDueWidgetPublisherWithExpoWidgets', () => {
  let publisher: ReturnType<typeof BillsDueWidgetPublisherWithExpoWidgets>;

  beforeEach(() => {
    jest.clearAllMocks();
    publisher = BillsDueWidgetPublisherWithExpoWidgets(display);
  });

  it('should publish one timeline entry at the start of each day', async () => {
    const result = await publisher.publish([
      { date: DateOnly.valid('2026-11-10'), summary: { pastDue: total(1, 20), dueToday: total(2, 12.5) } },
      { date: DateOnly.valid('2026-11-11'), summary: { pastDue: total(3, 32.5), dueToday: total(0, 0) } },
    ]);

    expect(result).toBeSuccessWithValue(undefined);
    expect(updateTimeline).toHaveBeenCalledWith([
      {
        date: new Date(2026, 10, 10),
        props: {
          state: 'ready',
          logo: 'file:///group/widgets/widget-tile.png',
          title: 'Bills',
          pastDue: { count: 1, amount: '$20', label: 'Past due' },
          dueToday: { count: 2, amount: '$12.5', label: 'Due today' },
        },
      },
      {
        date: new Date(2026, 10, 11),
        props: {
          state: 'ready',
          logo: 'file:///group/widgets/widget-tile.png',
          title: 'Bills',
          pastDue: { count: 3, amount: '$32.5', label: 'Past due' },
          dueToday: { count: 0, amount: '$0', label: 'Due today' },
        },
      },
    ]);
  });

  it('should show the signed out message when cleared', async () => {
    const result = await publisher.clear();

    expect(result).toBeSuccessWithValue(undefined);
    expect(updateSnapshot).toHaveBeenCalledWith({
      state: 'signed-out',
      logo: 'file:///group/widgets/widget-tile.png',
      message: 'Open the app',
    });
  });

  it('should fail when the timeline cannot be published', async () => {
    updateTimeline.mockImplementationOnce(() => {
      throw new Error('no app group');
    });

    const result = await publisher.publish([{ date: DateOnly.valid('2026-11-10'), summary: { pastDue: total(0, 0), dueToday: total(0, 0) } }]);

    expect(result).toBeFailureWithErrors([BillsDueWidgetPublisherErrors.publishFailed]);
  });

  it('should fail when the widget cannot be cleared', async () => {
    updateSnapshot.mockImplementationOnce(() => {
      throw new Error('no app group');
    });

    const result = await publisher.clear();

    expect(result).toBeFailureWithErrors([BillsDueWidgetPublisherErrors.publishFailed]);
  });
});
