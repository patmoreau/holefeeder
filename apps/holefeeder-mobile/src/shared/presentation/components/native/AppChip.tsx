import * as IosModifiers from '@expo/ui/swift-ui/modifiers';
import { Platform } from 'react-native';
import { useTheme } from '@/shared/theme/core/use-theme';
import { ExpoModifierConfig } from './expo/expo-modifiers';
import { ExpoButton } from './expo/ExpoButton';
import { ExpoText } from './expo/ExpoText';

export type AppChipProps = {
  label: string;
  selected?: boolean;
  onPress?: () => void;
};

export function AppChip({ label, selected = false, onPress }: AppChipProps) {
  const { theme } = useTheme();
  const buttonModifiers: ExpoModifierConfig[] = [];
  const textModifiers: ExpoModifierConfig[] = [];
  if (Platform.OS === 'ios') {
    buttonModifiers.push(
      IosModifiers.buttonStyle('bordered'),
      IosModifiers.controlSize('mini'),
      IosModifiers.tint(selected ? theme.colors.primary : theme.colors.secondary)
    );
    textModifiers.push(
      IosModifiers.font({ size: theme.typography.chip.fontSize }),
      IosModifiers.foregroundStyle(selected ? theme.colors.primary : theme.colors.secondary)
    );
  }

  return (
    <ExpoButton onPress={onPress} modifiers={buttonModifiers}>
      <ExpoText modifiers={textModifiers}>{label}</ExpoText>
    </ExpoButton>
  );
}
