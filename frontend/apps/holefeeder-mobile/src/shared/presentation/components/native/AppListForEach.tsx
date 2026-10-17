import { ExpoListForEach, type ExpoListForEachProps } from './expo/ExpoListForEach';

export type AppListForEachProps<T> = ExpoListForEachProps<T>;

export const AppListForEach = <T,>({ recycling = false, ...props }: AppListForEachProps<T>) => (
  <ExpoListForEach recycling={recycling} {...props} />
);
