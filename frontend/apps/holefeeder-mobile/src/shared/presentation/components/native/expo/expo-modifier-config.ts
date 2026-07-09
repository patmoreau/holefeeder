import { ModifierConfig as AndroidModifierConfig } from '@expo/ui/jetpack-compose/modifiers';
import { ModifierConfig as IosModifierConfig } from '@expo/ui/swift-ui/modifiers';

export type ExpoModifierConfig = IosModifierConfig | AndroidModifierConfig;
