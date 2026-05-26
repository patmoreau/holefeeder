import { Host, Icon } from '@expo/ui/jetpack-compose';
import type { SymbolWeight } from 'expo-symbols';
import { OpaqueColorValue } from 'react-native';
import { AppIcons, AppIconsMaterialMapping } from '@/shared/presentation/icons';

export const IconSymbol = ({
  name,
  size = 24,
  color,
}: {
  name: AppIcons;
  size?: number;
  color: string | OpaqueColorValue;
  style?: never;
  weight?: SymbolWeight;
}) => {
  return (
    <Host matchContents>
      <Icon source={AppIconsMaterialMapping[name]} size={size} tint={color as string} />
    </Host>
  );
};
