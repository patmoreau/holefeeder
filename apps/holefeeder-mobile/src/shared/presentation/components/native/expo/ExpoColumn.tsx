import { Column, ColumnProps } from '@expo/ui';

export type ExpoColumnProps = ColumnProps & {};

export const ExpoColumn = (props: ExpoColumnProps) => {
  return <Column {...props} />;
};
