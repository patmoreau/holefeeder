import { Stack } from 'expo-router';

const AuthLayout = () => {
  return (
    <Stack screenOptions={{ headerShown: false }}>
      <Stack.Screen name="Welcome" options={{ headerShown: false }} />
    </Stack>
  );
};

export default AuthLayout;
