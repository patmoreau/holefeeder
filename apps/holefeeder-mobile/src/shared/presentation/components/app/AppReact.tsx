import { ExpoRNHost, ExpoRNHostProps } from '@/shared/presentation/components/app/expo/ExpoRNHost';

export type AppReactProps = ExpoRNHostProps & {};

export const AppReact = (props: AppReactProps) => {
  return <ExpoRNHost {...props} />;
};
