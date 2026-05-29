import { ExpoFieldGroup, ExpoFieldGroupProps } from '@/shared/presentation/components/app/expo/ExpoFieldGroup';

export type AppFieldGroupProps = ExpoFieldGroupProps & {};

export const AppFieldGroup = (props: AppFieldGroupProps) => {
  return <ExpoFieldGroup {...props} />;
};
