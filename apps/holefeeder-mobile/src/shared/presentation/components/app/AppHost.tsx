import { ExpoHost, ExpoHostProps } from './expo/ExpoHost';

export type AppHostProps = ExpoHostProps & {};

export const AppHost = (props: AppHostProps) => {
  return <ExpoHost {...props} />;
};
