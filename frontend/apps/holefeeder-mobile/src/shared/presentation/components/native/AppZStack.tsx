import { ExpoZStack, ExpoZStackProps } from './expo/ExpoZStack';

export type AppZStackProps = ExpoZStackProps & {};

export const AppZStack = (props: AppZStackProps) => {
  return <ExpoZStack {...props} />;
};
