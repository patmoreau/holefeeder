import React, { createContext, ReactNode, useCallback, useContext, useState } from 'react';

type HeaderIconColorContextType = {
  isOverPrimary: boolean;
  setOverPrimary: (value: boolean) => void;
};

const HeaderIconColorContext = createContext<HeaderIconColorContextType>({
  isOverPrimary: true,
  setOverPrimary: () => {},
});

export const HeaderIconColorProvider = ({ children }: { children: ReactNode }) => {
  const [isOverPrimary, setIsOverPrimary] = useState(true);

  const setOverPrimary = useCallback((value: boolean) => {
    setIsOverPrimary(value);
  }, []);

  return <HeaderIconColorContext.Provider value={{ isOverPrimary, setOverPrimary }}>{children}</HeaderIconColorContext.Provider>;
};

export const useHeaderIconColor = () => useContext(HeaderIconColorContext);
