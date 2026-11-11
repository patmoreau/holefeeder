import { type ReactNode, useMemo, useState } from 'react';
import { ApiRefreshContext } from './api-refresh';

export const ApiRefreshProvider = ({ children }: { children: ReactNode }) => {
  const [version, setVersion] = useState(0);
  const value = useMemo(() => ({ version: version, refresh: () => setVersion((current) => current + 1) }), [version]);

  return <ApiRefreshContext.Provider value={value}>{children}</ApiRefreshContext.Provider>;
};
