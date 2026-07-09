import { Row, RowProps } from '@expo/ui';

export type ExpoRowProps = RowProps & {};

export const ExpoRow = (props: ExpoRowProps) => {
  return <Row {...props} />;
};
