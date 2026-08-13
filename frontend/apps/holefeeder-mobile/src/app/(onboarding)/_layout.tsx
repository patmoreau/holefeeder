import { Stack } from 'expo-router';

const OnboardingLayout = () => {
  return (
    <Stack screenOptions={{ headerShown: false }}>
      <Stack.Screen name="Registering" options={{ headerShown: false }} />
      {/* This one keeps its header: the budget step puts its save action in the toolbar. */}
      <Stack.Screen name="BudgetPeriod" options={{ headerShown: true, headerTransparent: true }} />
    </Stack>
  );
};

export default OnboardingLayout;
