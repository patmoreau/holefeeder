import { ExpoListForEach, type ExpoListForEachProps } from './expo/ExpoListForEach';

export type AppListForEachProps = ExpoListForEachProps & {};

export const AppListForEach = (props: AppListForEachProps) => <ExpoListForEach {...props} />;
