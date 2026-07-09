import { SwipeActions, type SwipeActionsGroupProps, type SwipeActionsProps } from '@expo/ui/swift-ui';

export type ExpoSwipeActionsProps = SwipeActionsProps;
export type ExpoSwipeActionsGroupProps = SwipeActionsGroupProps;

const ExpoSwipeActionsComponent = (props: ExpoSwipeActionsProps) => <SwipeActions {...props} />;

export const ExpoSwipeActions = Object.assign(ExpoSwipeActionsComponent, {
  Actions: SwipeActions.Actions,
});
