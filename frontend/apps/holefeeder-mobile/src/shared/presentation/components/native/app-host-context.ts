import { createContext, useContext } from 'react';

export const AppHostContext = createContext(false);
export const useIsInHost = () => useContext(AppHostContext);
