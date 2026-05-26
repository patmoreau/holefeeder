import { FieldGroup, FieldGroupProps } from '@expo/ui';
import { Form, FormProps } from '@expo/ui/swift-ui';

export type ExpoFieldGroupProps = FieldGroupProps & {};

export const ExpoFieldGroup = (props: ExpoFieldGroupProps) => {
  return <FieldGroup {...props} />;
};
