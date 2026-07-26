import { listStyle } from '@expo/ui/swift-ui/modifiers';
import { ExpoList, type ExpoListProps } from './expo/ExpoList';

export type AppListProps = ExpoListProps & {
  inset?: boolean;
};

export const AppList = ({ inset, modifiers = [], ...props }: AppListProps) => (
  <ExpoList {...props} modifiers={inset ? [...modifiers, listStyle('inset')] : modifiers} />
);
