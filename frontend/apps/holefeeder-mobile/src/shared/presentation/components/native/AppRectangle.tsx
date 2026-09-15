import { ExpoRectangle, ExpoRectangleProps } from './expo/ExpoRectangle';

export type AppRectangleProps = ExpoRectangleProps & {};

export const AppRectangle = (props: AppRectangleProps) => {
  return <ExpoRectangle {...props} />;
};
