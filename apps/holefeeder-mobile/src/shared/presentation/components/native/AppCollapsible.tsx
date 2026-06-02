import { ExpoCollapsible, ExpoCollapsibleProps } from '@/shared/presentation/components/native/expo/ExpoCollapsible';

export type AppCollapsibleProps = ExpoCollapsibleProps & {};

export const AppCollapsible = (props: AppCollapsibleProps) => {
  return <ExpoCollapsible {...props} />;
};
