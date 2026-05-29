import { FieldGroup, FieldGroupProps } from '@expo/ui';

export type ExpoFieldGroupProps = FieldGroupProps & {};

export const ExpoFieldGroup = (props: ExpoFieldGroupProps) => {
  return <FieldGroup {...props} />;
};
