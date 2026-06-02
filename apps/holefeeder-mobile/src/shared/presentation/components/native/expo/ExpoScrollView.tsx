import { ScrollView, ScrollViewProps } from '@expo/ui';

export type ExpoScrollViewProps = ScrollViewProps & {};

export const ExpoScrollView = (props: ExpoScrollViewProps) => {
  return <ScrollView {...props} />;
};
