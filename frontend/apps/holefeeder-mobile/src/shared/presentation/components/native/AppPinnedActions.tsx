import * as IosModifiers from '@expo/ui/swift-ui/modifiers';
import { Platform, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { ExpoModifierConfig } from '@/shared/presentation/components/native/expo/expo-modifier-config';
import { useKeyboardHeight } from '@/shared/presentation/core/use-keyboard-height';
import { useTheme } from '@/shared/theme/core/use-theme';
import { spacing } from '@/types/theme/design-tokens';
import { AppNative } from './AppNative';
import { ExpoButton } from './expo/ExpoButton';
import { ExpoColumn } from './expo/ExpoColumn';
import { ExpoText } from './expo/ExpoText';

export type PinnedAction = {
  label: string;
  onPress: () => void;
  testID?: string;
};

export type AppPinnedActionsProps = {
  primary: PinnedAction;
  secondary?: PinnedAction;
  disabled?: boolean;
};

export const AppPinnedActions = ({ primary, secondary, disabled }: AppPinnedActionsProps) => {
  const { theme } = useTheme();
  const insets = useSafeAreaInsets();
  const keyboardHeight = useKeyboardHeight();
  const isIos = Platform.OS === 'ios';

  const primaryModifiers: ExpoModifierConfig[] = isIos
    ? [IosModifiers.buttonStyle('glassProminent'), IosModifiers.controlSize('large'), IosModifiers.tint(theme.colors.primary)]
    : [];
  const labelModifiers: ExpoModifierConfig[] = isIos ? [IosModifiers.frame({ maxWidth: Infinity })] : [];
  const secondaryModifiers: ExpoModifierConfig[] = isIos
    ? [IosModifiers.buttonStyle('plain'), IosModifiers.foregroundStyle(theme.colors.primary)]
    : [];

  return (
    <View
      style={{
        paddingHorizontal: spacing.xl,
        paddingTop: spacing.md,
        paddingBottom: keyboardHeight > 0 ? spacing.md : Math.max(insets.bottom, spacing.md),
        marginBottom: keyboardHeight,
        backgroundColor: theme.colors.secondaryBackground,
      }}
    >
      <AppNative matchContents={{ vertical: true }}>
        <ExpoColumn spacing={spacing.xs}>
          <ExpoButton onPress={primary.onPress} disabled={disabled} modifiers={primaryModifiers} testID={primary.testID}>
            <ExpoText modifiers={labelModifiers}>{primary.label}</ExpoText>
          </ExpoButton>
          {secondary && (
            <ExpoButton
              label={secondary.label}
              onPress={secondary.onPress}
              disabled={disabled}
              modifiers={secondaryModifiers}
              testID={secondary.testID}
            />
          )}
        </ExpoColumn>
      </AppNative>
    </View>
  );
};
