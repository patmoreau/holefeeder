import { type ScrollViewProps } from 'react-native';
import { AppFieldGroup } from './AppFieldGroup';
import { AppHost } from './AppHost';

export const AppForm = ({ style, children, ...otherProps }: ScrollViewProps) => {
  return (
    <AppHost style={{ flex: 1 }}>
      <AppFieldGroup {...otherProps}>{children}</AppFieldGroup>
    </AppHost>
  );
};
