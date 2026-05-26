import { Host, UniversalHostProps } from '@expo/ui';

export type ExpoHostProps = UniversalHostProps & {};

export const ExpoHost = (props: ExpoHostProps) => {
  return <Host {...props} />;
};
