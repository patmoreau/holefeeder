import { ZStack, ZStackProps } from '@expo/ui/swift-ui';

export type ExpoZStackProps = ZStackProps & {};

export const ExpoZStack = (props: ExpoZStackProps) => {
  return <ZStack {...props} />;
};
