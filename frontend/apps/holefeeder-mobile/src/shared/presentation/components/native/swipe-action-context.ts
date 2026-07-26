import { createContext, useContext } from 'react';

// True while rendering inside an AppSwipeActions.Actions group. SwiftUI's
// .swipeActions only renders plain Buttons reliably, so AppButton drops its
// style modifiers (buttonStyle/foregroundStyle/labelStyle) in this context.
export const SwipeActionContext = createContext(false);

export const useInSwipeAction = () => useContext(SwipeActionContext);
