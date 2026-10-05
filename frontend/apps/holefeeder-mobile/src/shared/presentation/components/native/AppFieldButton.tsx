import { buttonStyle, foregroundStyle, frame } from '@expo/ui/swift-ui/modifiers';
import { Platform } from 'react-native';
import { useTheme } from '@/shared/theme/core/use-theme';
import { ExpoButton } from './expo/ExpoButton';
import { ExpoText } from './expo/ExpoText';

export type AppFieldButtonProps = {
  label: string;
  onPress: () => void;
  destructive?: boolean;
  testID?: string;
};

export const AppFieldButton = ({ label, onPress, destructive = false, testID }: AppFieldButtonProps) => {
  const { theme } = useTheme();

  if (Platform.OS !== 'ios') {
    return <ExpoButton label={label} onPress={onPress} testID={testID} />;
  }

  return (
    <ExpoButton
      role={destructive ? 'destructive' : undefined}
      onPress={onPress}
      modifiers={[buttonStyle('plain'), foregroundStyle(destructive ? theme.colors.destructive : theme.colors.primary)]}
      testID={testID}
    >
      <ExpoText modifiers={[frame({ maxWidth: Infinity })]}>{label}</ExpoText>
    </ExpoButton>
  );
};
