import { ViewProps } from 'react-native';
import { AppView } from '@/shared/presentation/AppView';

export const AppModal = ({ style, children, ...otherProps }: ViewProps) => {
  return (
    <AppView style={[{ flex: 1 }, style]} {...otherProps}>
      {children}
    </AppView>
  );
};
