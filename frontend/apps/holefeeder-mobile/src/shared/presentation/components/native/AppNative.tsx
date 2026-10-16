import { useWindowDimensions } from 'react-native';
import { ExpoHost, ExpoHostProps } from './expo/ExpoHost';

export type AppHostProps = ExpoHostProps & {};

export const AppNative = ({ children, matchContents, alignment = 'center', ...props }: AppHostProps) => {
  const { fontScale } = useWindowDimensions();

  return (
    <ExpoHost key={matchContents ? fontScale : undefined} matchContents={matchContents} alignment={alignment} {...props}>
      {children}
    </ExpoHost>
  );
};
