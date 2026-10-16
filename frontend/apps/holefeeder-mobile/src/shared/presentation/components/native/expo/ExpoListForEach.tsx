import { ListForEach, type ListForEachElementsProps } from '@expo/ui/swift-ui';

export type ExpoListForEachProps = ListForEachElementsProps;

export const ExpoListForEach = (props: ExpoListForEachProps) => <ListForEach {...props} />;
