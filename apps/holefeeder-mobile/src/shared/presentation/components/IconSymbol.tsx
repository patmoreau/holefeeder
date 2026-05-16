import { MaterialIcons } from '@expo/vector-icons';
import type { SymbolWeight } from 'expo-symbols';
import { OpaqueColorValue, type StyleProp, type TextStyle } from 'react-native';
import { AppIcons, AppIconsMapping } from '@/shared/presentation/icons';

export const IconSymbol = ({
  name,
  size = 24,
  color,
  style,
}: {
  name: AppIcons;
  size?: number;
  color: string | OpaqueColorValue;
  style?: StyleProp<TextStyle>;
  weight?: SymbolWeight;
}) => {
  return <MaterialIcons color={color} size={size} name={AppIconsMapping[name]} style={style} />;
};
