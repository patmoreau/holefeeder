// types/global.d.ts
declare global {
  namespace NodeJS {
    interface Global {
      ErrorUtils: {
        setGlobalHandler: (handler: (error: Error, isFatal: boolean) => void) => void;
      };
    }
  }
}

export {};
