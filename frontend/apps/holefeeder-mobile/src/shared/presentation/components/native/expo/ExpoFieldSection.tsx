import { FieldGroup, FieldSectionProps } from '@expo/ui';

export type ExpoFieldSectionProps = FieldSectionProps & {};

export const ExpoFieldSection = (props: ExpoFieldSectionProps) => {
  return <FieldGroup.Section {...props} />;
};
