import { ExpoColumn, ExpoColumnProps } from './expo/ExpoColumn';

export type AppColumnProps = ExpoColumnProps & {};

export const AppColumn = ({ spacing = 0, ...props }: AppColumnProps) => {
  return <ExpoColumn spacing={spacing} {...props} />;
};
