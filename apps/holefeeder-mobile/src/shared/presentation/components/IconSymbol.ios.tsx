import { SymbolView, type SymbolWeight } from 'expo-symbols';
import { StyleProp, ViewStyle } from 'react-native';
import { AppIcons } from '@/shared/presentation/icons';

export const IconSymbol = ({
  name,
  size = 24,
  color,
  style,
  weight = 'regular',
}: {
  name: AppIcons;
  size?: number;
  color: string;
  style?: StyleProp<ViewStyle>;
  weight?: SymbolWeight;
}) => {
  return (
    <SymbolView
      weight={weight}
      tintColor={color}
      resizeMode="scaleAspectFit"
      name={name}
      style={[
        {
          width: size,
          height: size,
        },
        style,
      ]}
    />
  );
};
