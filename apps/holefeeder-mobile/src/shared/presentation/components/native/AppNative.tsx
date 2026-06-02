import { ExpoHost, ExpoHostProps } from './expo/ExpoHost';

export type AppHostProps = ExpoHostProps & {};

export const AppNative = ({ children, ...props }: AppHostProps) => {
  return <ExpoHost {...props}>{children}</ExpoHost>;
};
