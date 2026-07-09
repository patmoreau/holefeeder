import { ExpoFieldGroup, ExpoFieldGroupProps } from '@/shared/presentation/components/native/expo/ExpoFieldGroup';

export type AppFieldGroupProps = ExpoFieldGroupProps & {};

export const AppFieldGroup = (props: AppFieldGroupProps) => {
  return <ExpoFieldGroup {...props} />;
};
