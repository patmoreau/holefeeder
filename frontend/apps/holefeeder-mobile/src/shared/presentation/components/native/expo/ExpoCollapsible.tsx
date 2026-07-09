import { Collapsible, CollapsibleProps } from '@expo/ui';

export type ExpoCollapsibleProps = CollapsibleProps & {};

export const ExpoCollapsible = (props: ExpoCollapsibleProps) => {
  return <Collapsible {...props} />;
};
