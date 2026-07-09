import { ExpoList, type ExpoListProps } from './expo/ExpoList';

export type AppListProps = ExpoListProps & {};

export const AppList = (props: AppListProps) => <ExpoList {...props} />;
