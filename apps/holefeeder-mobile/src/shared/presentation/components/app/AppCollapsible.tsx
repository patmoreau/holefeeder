import { ExpoCollapsible, ExpoCollapsibleProps } from '@/shared/presentation/components/app/expo/ExpoCollapsible';

export type AppCollapsibleProps = ExpoCollapsibleProps & {};

export const AppCollapsible = (props: AppCollapsibleProps) => {
  return <ExpoCollapsible {...props} />;
};
