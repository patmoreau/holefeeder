import { Spacer, SpacerProps } from '@expo/ui';

export type ExpoSpacerProps = SpacerProps & {};

export const ExpoSpacer = (props: ExpoSpacerProps) => {
  return <Spacer {...props} />;
};
