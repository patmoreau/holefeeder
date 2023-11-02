import { inject } from '@angular/core';
import { LoggerService } from '@app/core/logger';
import { LoggingLevel } from '@app/shared/models/logging-level.enum';
import ignoreDuringUnitTest from './ignore-during-unit-test.decorator';

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

class TraceService {
  constructor() {
    this.decoratorFactory = this.decoratorFactory.bind(this);
  }

  @ignoreDuringUnitTest()
  decorator(
    target: unknown,
    propertyKey: string,
    descriptor: PropertyDescriptor
  ) {
    const logger = inject(LoggerService);
    const targetMethod = descriptor.value;
    const options = defaultParams;

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
  }

  decoratorFactory() {
    return this.decorator;
  }
}

const trace = new TraceService().decoratorFactory;
export default trace;
