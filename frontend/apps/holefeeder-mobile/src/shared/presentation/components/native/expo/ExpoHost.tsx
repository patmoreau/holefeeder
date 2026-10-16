import { Host, type HostProps } from '@expo/ui/swift-ui';

export type ExpoHostProps = HostProps & {};

export const ExpoHost = (props: ExpoHostProps) => {
  return <Host {...props} />;
};
