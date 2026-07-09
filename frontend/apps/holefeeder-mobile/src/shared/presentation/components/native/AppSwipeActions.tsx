import {
  ExpoSwipeActions,
  type ExpoSwipeActionsGroupProps,
  type ExpoSwipeActionsProps,
} from '@/shared/presentation/components/native/expo/ExpoSwipeActions';

export type AppSwipeActionsProps = ExpoSwipeActionsProps & {};
export type AppSwipeActionsGroupProps = ExpoSwipeActionsGroupProps;

const AppSwipeActionsComponent = (props: AppSwipeActionsProps) => <ExpoSwipeActions {...props} />;

export const AppSwipeActions = Object.assign(AppSwipeActionsComponent, {
  Actions: ExpoSwipeActions.Actions,
});
