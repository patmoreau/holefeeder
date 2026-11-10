import { FieldGroup, FieldSectionFooterProps } from '@expo/ui';

export type ExpoFieldSectionFooterProps = FieldSectionFooterProps & {};

export const ExpoFieldSectionFooter = (props: ExpoFieldSectionFooterProps) => {
  return <FieldGroup.SectionFooter {...props} />;
};
