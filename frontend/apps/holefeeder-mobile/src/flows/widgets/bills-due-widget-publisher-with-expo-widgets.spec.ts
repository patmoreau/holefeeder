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
const total = (count: number, expenses: number, gains: number) => ({
  count: count,
  expenses: Money.valid(expenses),
  gains: Money.valid(gains),
});
const today = DateOnly.valid('2026-11-10');

describe('BillsDueWidgetPublisherWithExpoWidgets', () => {
  let publisher: ReturnType<typeof BillsDueWidgetPublisherWithExpoWidgets>;

  const publishedPastDue = async (pastDue: ReturnType<typeof total>) => {
    await publisher.publish([{ date: today, summary: { pastDue: pastDue, dueToday: total(0, 0, 0) } }]);
    const props = updateTimeline.mock.calls[0][0][0].props;
    return props.state === 'ready' ? props.pastDue : undefined;
  };

  beforeEach(() => {
    jest.clearAllMocks();
    publisher = BillsDueWidgetPublisherWithExpoWidgets(display);
  });

  it('should publish one timeline entry at the start of each day', async () => {
    const result = await publisher.publish([
      { date: DateOnly.valid('2026-11-10'), summary: { pastDue: total(1, 20, 0), dueToday: total(2, 12.5, 0) } },
      { date: DateOnly.valid('2026-11-11'), summary: { pastDue: total(3, 32.5, 0), dueToday: total(0, 0, 0) } },
    ]);

    expect(result).toBeSuccessWithValue(undefined);
    expect(updateTimeline).toHaveBeenCalledWith([
      {
        date: new Date(2026, 10, 10),
        props: {
          state: 'ready',
          logo: 'file:///group/widgets/widget-tile.png',
          title: 'Bills',
          pastDue: { count: 1, label: 'Past due', amounts: [{ direction: 'down', text: '$20' }] },
          dueToday: { count: 2, label: 'Due today', amounts: [{ direction: 'down', text: '$12.5' }] },
        },
      },
      {
        date: new Date(2026, 10, 11),
        props: {
          state: 'ready',
          logo: 'file:///group/widgets/widget-tile.png',
          title: 'Bills',
          pastDue: { count: 3, label: 'Past due', amounts: [{ direction: 'down', text: '$32.5' }] },
          dueToday: { count: 0, label: 'Due today', amounts: [{ direction: 'none', text: '$0' }] },
        },
      },
    ]);
  });

  it('should show gains with an up arrow', async () => {
    expect((await publishedPastDue(total(1, 0, 40)))?.amounts).toEqual([{ direction: 'up', text: '$40' }]);
  });

  it('should show expenses before gains when there are both', async () => {
    expect((await publishedPastDue(total(2, 15, 40)))?.amounts).toEqual([
      { direction: 'down', text: '$15' },
      { direction: 'up', text: '$40' },
    ]);
  });

  it('should show zero without an arrow when there is nothing to pay or receive', async () => {
    expect((await publishedPastDue(total(0, 0, 0)))?.amounts).toEqual([{ direction: 'none', text: '$0' }]);
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

    const result = await publisher.publish([{ date: today, summary: { pastDue: total(0, 0, 0), dueToday: total(0, 0, 0) } }]);

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
