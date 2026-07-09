import { Icon, IconProps } from '@expo/ui';

export type ExpoIconProps = IconProps & {};

export const ExpoIcon = (props: ExpoIconProps) => {
  return <Icon {...props} />;
};
