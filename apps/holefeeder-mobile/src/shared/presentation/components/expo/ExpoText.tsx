import { Text, TextProps } from '@expo/ui';

export type ExpoTextProps = TextProps & {};

export const ExpoText = (props: ExpoTextProps) => {
  return <Text {...props} />;
};
