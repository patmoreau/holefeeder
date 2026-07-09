import { ExpoIcon, ExpoIconProps } from './expo/ExpoIcon';

export type AppIconProps = ExpoIconProps & {};

export const AppIcon = (props: AppIconProps) => {
  return <ExpoIcon {...props} />;
};
