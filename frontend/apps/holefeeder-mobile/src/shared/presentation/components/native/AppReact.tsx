import { useWindowDimensions } from 'react-native';
import { ExpoRNHost, ExpoRNHostProps } from '@/shared/presentation/components/native/expo/ExpoRNHost';

export type AppReactProps = ExpoRNHostProps & {};

export const AppReact = ({ matchContents, ...props }: AppReactProps) => {
  const { fontScale } = useWindowDimensions();

  return <ExpoRNHost key={matchContents ? fontScale : undefined} matchContents={matchContents} {...props} />;
};
