import { ConsoleLogger, Logger, LoggingLevel } from './logger.service';

export const logger: Logger = ConsoleLogger.getInstance();

interface LoggerParams {
  type?:
    | LoggingLevel.Verbose
    | LoggingLevel.Debug
    | LoggingLevel.Info
    | LoggingLevel.Warning
    | LoggingLevel.Error;
  inputs?: boolean;
  outputs?: boolean;
}

const defaultParams: Required<LoggerParams> = {
  type: LoggingLevel.Debug,
  inputs: true,
  outputs: false,
};

export function trace(params?: LoggerParams) {
  const options: Required<LoggerParams> = {
    type: params?.type || defaultParams.type,
    inputs: params?.inputs ?? defaultParams.inputs,
    outputs: params?.outputs ?? defaultParams.outputs,
  };

  return function (
    target: any,
    propertyKey: string,
    descriptor: PropertyDescriptor
  ) {
    const targetMethod = descriptor.value;

    descriptor.value = function (...args: any[]) {
      if (options.inputs && args.length) {
        logger.log(options.type, `Tracing ${propertyKey} <=`, args);
      } else {
        logger.log(options.type, `Tracing ${propertyKey}`);
      }
      const result = targetMethod.apply(this, args);

      if (options.outputs) {
        logger.log(options.type, `Tracing ${propertyKey} =>`, result);
      }

      return result;
    };
    return descriptor;
  };
}
