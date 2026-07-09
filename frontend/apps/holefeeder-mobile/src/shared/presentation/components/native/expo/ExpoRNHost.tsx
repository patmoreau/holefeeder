import { RNHostView, RNHostViewProps } from '@expo/ui';

export type ExpoRNHostProps = RNHostViewProps & {};

export const ExpoRNHost = (props: RNHostViewProps) => {
  return <RNHostView {...props} />;
};
