import { frame } from '@expo/ui/swift-ui/modifiers';
import { Platform } from 'react-native';
import { componentSizes } from '@/types/theme/design-tokens';
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

export const minTouchTargetModifier = () => frame({ minHeight: componentSizes.minTouchTarget });

const AppListItemComponent = ({ modifiers = [], ...props }: AppListItemProps) => (
  <ExpoListItem {...props} modifiers={Platform.OS === 'ios' ? [minTouchTargetModifier(), ...modifiers] : modifiers} />
);

export const AppListItem = Object.assign(AppListItemComponent, {
  Leading: ExpoListItem.Leading,
  Trailing: ExpoListItem.Trailing,
  Supporting: ExpoListItem.Supporting,
});
