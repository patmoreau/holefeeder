import { ExpoScrollView, ExpoScrollViewProps } from '@/shared/presentation/components/native/expo/ExpoScrollView';

export type AppScrollViewProps = ExpoScrollViewProps & {};

export const AppScrollView = (props: AppScrollViewProps) => {
  return <ExpoScrollView {...props} />;
};
