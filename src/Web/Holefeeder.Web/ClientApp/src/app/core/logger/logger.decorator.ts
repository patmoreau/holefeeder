import { DecoratorLogger, Logger } from './logger.service';
import { LoggingLevel } from './logging-level.enum';

export const logger: Logger = DecoratorLogger.getInstance();

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
    target: unknown,
    propertyKey: string,
    descriptor: PropertyDescriptor
  ) {
    const targetMethod = descriptor.value;

    descriptor.value = function (...args: unknown[]) {
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
