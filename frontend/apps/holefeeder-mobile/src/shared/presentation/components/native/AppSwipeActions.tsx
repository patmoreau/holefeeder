import {
  ExpoSwipeActions,
  type ExpoSwipeActionsGroupProps,
  type ExpoSwipeActionsProps,
} from '@/shared/presentation/components/native/expo/ExpoSwipeActions';
import { SwipeActionContext } from '@/shared/presentation/components/native/swipe-action-context';

export type AppSwipeActionsProps = ExpoSwipeActionsProps & {};
export type AppSwipeActionsGroupProps = ExpoSwipeActionsGroupProps;

const AppSwipeActionsComponent = (props: AppSwipeActionsProps) => <ExpoSwipeActions {...props} />;

// Flags descendant AppButtons that they render inside a swipe action group.
const AppSwipeActionsGroup = ({ children, ...props }: AppSwipeActionsGroupProps) => (
  <ExpoSwipeActions.Actions {...props}>
    <SwipeActionContext.Provider value={true}>{children}</SwipeActionContext.Provider>
  </ExpoSwipeActions.Actions>
);

export const AppSwipeActions = Object.assign(AppSwipeActionsComponent, {
  Actions: AppSwipeActionsGroup,
});
