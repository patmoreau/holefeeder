import { SpendingWidgetPublisherErrors } from '@/dashboard/core/publish-spending-widget/spending-widget-publisher';
import { SpendingWidgetPublisherWithExpoWidgets } from '@/dashboard/widgets/spending-widget-publisher-with-expo-widgets';
import SpendingWidget from '@/dashboard/widgets/SpendingWidget';

const updateSnapshot = jest.mocked(SpendingWidget.updateSnapshot);

const display = {
  formatAmount: (amount: number) => `$${amount}`,
  formatTrend: (amount: string) => `${amount} vs avg`,
  labels: { title: 'Spending', projected: 'Projected', openApp: 'Open the app' },
  logo: 'file:///group/widgets/widget-tile.png',
};

describe('SpendingWidgetPublisherWithExpoWidgets', () => {
  let publisher: ReturnType<typeof SpendingWidgetPublisherWithExpoWidgets>;

  const publishedProps = () => updateSnapshot.mock.calls[0][0];

  beforeEach(() => {
    jest.clearAllMocks();
    publisher = SpendingWidgetPublisherWithExpoWidgets(display);
  });

  it('should publish the spending, the trend and the projected net flow', async () => {
    const result = await publisher.publish({ spending: 1847.63, trend: { amount: 212.4, isOver: false }, projected: 317.92 });

    expect(result).toBeSuccessWithValue(undefined);
    expect(updateSnapshot).toHaveBeenCalledWith({
      state: 'ready',
      logo: 'file:///group/widgets/widget-tile.png',
      title: 'Spending',
      spending: '$1847.63',
      trend: { text: '$212.4 vs avg', direction: 'down' },
      projected: { label: 'Projected', text: '$317.92', tone: 'positive' },
    });
  });

  it('should point the trend up when spending is over the average', async () => {
    await publisher.publish({ spending: 300, trend: { amount: 40, isOver: true }, projected: 0 });

    const props = publishedProps();
    expect(props.state === 'ready' && props.trend).toEqual({ text: '$40 vs avg', direction: 'up' });
  });

  it('should leave the trend out when there is none', async () => {
    await publisher.publish({ spending: 50, projected: 0 });

    const props = publishedProps();
    expect(props.state === 'ready' && props.trend).toBeUndefined();
  });

  it('should show a projected shortfall as an amount with a negative tone', async () => {
    await publisher.publish({ spending: 50, projected: -120.5 });

    const props = publishedProps();
    expect(props.state === 'ready' && props.projected).toEqual({ label: 'Projected', text: '$120.5', tone: 'negative' });
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

  it('should fail when the widget cannot be published', async () => {
    updateSnapshot.mockImplementationOnce(() => {
      throw new Error('no app group');
    });

    const result = await publisher.publish({ spending: 0, projected: 0 });

    expect(result).toBeFailureWithErrors([SpendingWidgetPublisherErrors.publishFailed]);
  });

  it('should fail when the widget cannot be cleared', async () => {
    updateSnapshot.mockImplementationOnce(() => {
      throw new Error('no app group');
    });

    const result = await publisher.clear();

    expect(result).toBeFailureWithErrors([SpendingWidgetPublisherErrors.publishFailed]);
  });
});
