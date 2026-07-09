import { ExpoRow, ExpoRowProps } from './expo/ExpoRow';

export type AppRowProps = ExpoRowProps & {};

export const AppRow = (props: AppRowProps) => {
  return <ExpoRow {...props} />;
};
