import * as IosModifiers from '@expo/ui/swift-ui/modifiers';
import { Platform } from 'react-native';
import { ExpoModifierConfig } from '@/shared/presentation/components/native/expo/expo-modifier-config';
import { useTheme } from '@/shared/theme/core/use-theme';
import { ExpoButton } from './expo/ExpoButton';
import { ExpoText } from './expo/ExpoText';

export type AppChipProps = {
  label: string;
  selected?: boolean;
  filled?: boolean;
  onPress?: () => void;
  testID?: string;
};

export const chipFontModifier = () => IosModifiers.font({ textStyle: 'footnote' });

export function AppChip({ label, selected = false, filled = false, onPress, testID }: AppChipProps) {
  const { theme } = useTheme();
  const buttonModifiers: ExpoModifierConfig[] = [];
  const textModifiers: ExpoModifierConfig[] = [];
  if (Platform.OS === 'ios') {
    const color = selected ? theme.colors.primary : theme.colors.secondaryText;
    const prominent = selected && filled;
    buttonModifiers.push(
      IosModifiers.buttonStyle(prominent ? 'borderedProminent' : 'bordered'),
      IosModifiers.controlSize('mini'),
      IosModifiers.tint(color)
    );
    textModifiers.push(chipFontModifier(), IosModifiers.foregroundStyle(prominent ? theme.colors.primaryText : color));
  }

  return (
    <ExpoButton onPress={onPress} modifiers={buttonModifiers} testID={testID}>
      <ExpoText modifiers={textModifiers}>{label}</ExpoText>
    </ExpoButton>
  );
}
