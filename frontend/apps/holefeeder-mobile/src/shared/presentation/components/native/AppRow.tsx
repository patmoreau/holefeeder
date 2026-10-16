import { ExpoRow, ExpoRowProps } from './expo/ExpoRow';

export type AppRowProps = ExpoRowProps & {};

export const AppRow = ({ spacing = 0, ...props }: AppRowProps) => {
  return <ExpoRow spacing={spacing} {...props} />;
};
