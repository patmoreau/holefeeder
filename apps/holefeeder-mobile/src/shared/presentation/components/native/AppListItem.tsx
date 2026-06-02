import {
  ExpoListItem,
  type ExpoListItemLeadingProps,
  type ExpoListItemProps,
  type ExpoListItemSupportingProps,
  type ExpoListItemTrailingProps,
} from './expo/ExpoListItem';

export type AppListItemProps = ExpoListItemProps & {};
export type AppListItemLeadingProps = ExpoListItemLeadingProps;
export type AppListItemTrailingProps = ExpoListItemTrailingProps;
export type AppListItemSupportingProps = ExpoListItemSupportingProps;

const AppListItemComponent = (props: AppListItemProps) => <ExpoListItem {...props} />;

export const AppListItem = Object.assign(AppListItemComponent, {
  Leading: ExpoListItem.Leading,
  Trailing: ExpoListItem.Trailing,
  Supporting: ExpoListItem.Supporting,
});
