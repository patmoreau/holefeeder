import { ListItem, type ListItemLeadingProps, type ListItemProps, type ListItemSupportingProps, type ListItemTrailingProps } from '@expo/ui';

export type ExpoListItemProps = ListItemProps;
export type ExpoListItemLeadingProps = ListItemLeadingProps;
export type ExpoListItemTrailingProps = ListItemTrailingProps;
export type ExpoListItemSupportingProps = ListItemSupportingProps;

const ExpoListItemComponent = (props: ExpoListItemProps) => <ListItem {...props} />;

export const ExpoListItem = Object.assign(ExpoListItemComponent, {
  Leading: ListItem.Leading,
  Trailing: ListItem.Trailing,
  Supporting: ListItem.Supporting,
});
