class IgnoreDuringUnitTestService {
  constructor() {
    this.decoratorFactory = this.decoratorFactory.bind(this);
  }

  decorator(
    target: unknown,
    propertyKey: string,
    descriptor: PropertyDescriptor
  ) {
    const targetMethod = descriptor.value;

    descriptor.value = function () {
      // eslint-disable-next-line @typescript-eslint/ban-ts-comment
      // @ts-ignore - jasmine is a global variable
      if (window.jasmine) {
        return descriptor;
      }
      return targetMethod(target, propertyKey, descriptor);
    };
    return descriptor;
  }

  decoratorFactory() {
    return this.decorator;
  }
}

const ignoreDuringUnitTest = new IgnoreDuringUnitTestService().decoratorFactory;
export default ignoreDuringUnitTest;
