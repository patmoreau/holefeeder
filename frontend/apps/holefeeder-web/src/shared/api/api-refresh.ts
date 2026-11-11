import { createContext, useContext } from 'react';

export type ApiRefresh = {
  version: number;
  refresh: () => void;
};

export const ApiRefreshContext = createContext<ApiRefresh>({ version: 0, refresh: () => {} });

export const useApiRefresh = () => useContext(ApiRefreshContext);
