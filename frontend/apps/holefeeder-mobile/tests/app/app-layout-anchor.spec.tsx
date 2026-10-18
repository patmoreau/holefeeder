import { router, Stack } from 'expo-router';
import { act, renderRouter, screen } from 'expo-router/testing-library';
import { Text } from 'react-native';
import { unstable_settings as appLayoutSettings } from '@/app/(app)/_layout';

const routes = {
  _layout: () => <Stack />,
  '(auth)/_layout': () => <Stack />,
  '(auth)/Welcome': () => <Text>Welcome</Text>,
  '(app)/_layout': { default: () => <Stack />, unstable_settings: appLayoutSettings },
  '(app)/(tabs)/_layout': () => <Stack />,
  '(app)/(tabs)/index': () => <Text>Dashboard</Text>,
  '(app)/Purchase': () => <Text>Purchase</Text>,
};

describe('(app) layout', () => {
  it('puts the dashboard under a screen opened from outside the app group', async () => {
    await renderRouter(routes, { initialUrl: '/Welcome' });

    await act(async () => router.navigate('/(app)/Purchase', { withAnchor: true }));
    expect(screen.getByText('Purchase')).toBeVisible();
    expect(router.canGoBack()).toBe(true);

    await act(async () => router.back());
    expect(screen.getByText('Dashboard')).toBeVisible();
  });
});
