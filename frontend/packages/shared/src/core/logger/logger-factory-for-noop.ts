import { LoggerFactory } from './logger';

export const loggerFactoryForNoop: LoggerFactory = () => ({
  debug: () => {},
  info: () => {},
  warn: () => {},
  error: () => {},
});
