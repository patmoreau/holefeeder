import { type ScrollViewProps } from 'react-native';
import { AppHostProps, AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppFieldGroup } from './AppFieldGroup';

export type AppFormProps = ScrollViewProps & {
  hostProps?: Omit<AppHostProps, 'children'>;
};

export const AppForm = ({ style, children, hostProps, ...otherProps }: AppFormProps) => {
  return (
    <AppNative style={{ flex: 1 }} {...hostProps}>
      <AppFieldGroup {...otherProps}>{children}</AppFieldGroup>
    </AppNative>
  );
};
