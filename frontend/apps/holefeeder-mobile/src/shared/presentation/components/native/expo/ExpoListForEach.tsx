import { ListForEach, type ListForEachProps } from '@expo/ui/swift-ui';

export type ExpoListForEachProps<T> = ListForEachProps<T>;

export const ExpoListForEach = <T,>(props: ExpoListForEachProps<T>) => <ListForEach {...props} />;
