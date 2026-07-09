import { loggerFactoryForNoop } from './logger-factory-for-noop';

type Logger = {
  debug: (...args: unknown[]) => void;
  info: (...args: unknown[]) => void;
  warn: (...args: unknown[]) => void;
  error: (...args: unknown[]) => void;
};

export type LoggerFactory = (module: string) => Logger;

let factory: LoggerFactory = loggerFactoryForNoop;

const setLoggerFactory = (newFactory: LoggerFactory) => (factory = newFactory);

const create = (module: string): Logger => ({
  debug: (...args: unknown[]) => factory(module).debug(...args),
  info: (...args: unknown[]) => factory(module).info(...args),
  warn: (...args: unknown[]) => factory(module).warn(...args),
  error: (...args: unknown[]) => factory(module).error(...args),
});

export const Logger = {
  setLoggerFactory: setLoggerFactory,
  create: create,
};
