import { ExpoColumn, ExpoColumnProps } from './expo/ExpoColumn';

export type AppColumnProps = ExpoColumnProps & {};

export const AppColumn = (props: AppColumnProps) => {
  return <ExpoColumn {...props} />;
};
