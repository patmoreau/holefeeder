import { List, type ListProps } from '@expo/ui/swift-ui';

export type ExpoListProps = ListProps;

export const ExpoList = (props: ExpoListProps) => <List {...props} />;
