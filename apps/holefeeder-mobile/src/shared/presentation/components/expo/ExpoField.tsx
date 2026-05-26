import { FieldGroup, FieldGroupProps } from '@expo/ui';

export type AppFieldGroupProps = FieldGroupProps & {};

export const AppFieldGroup = (props: AppFieldGroupProps) => {
  return <FieldGroup {...props} />;
};
