import { Rectangle, RectangleProps } from '@expo/ui/swift-ui';

export type ExpoRectangleProps = RectangleProps & {};

export const ExpoRectangle = (props: ExpoRectangleProps) => {
  return <Rectangle {...props} />;
};
