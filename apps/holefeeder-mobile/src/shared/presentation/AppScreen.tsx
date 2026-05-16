import { useHeaderHeight } from '@react-navigation/elements';
import { ViewProps } from 'react-native';
import { AppView } from '@/shared/presentation/AppView';

export const AppScreen = ({ style, children, ...otherProps }: ViewProps) => {
  const headerHeight = useHeaderHeight();

  return (
    <AppView style={[{ flex: 1, paddingTop: headerHeight }, style]} {...otherProps}>
      {children}
    </AppView>
  );
};
