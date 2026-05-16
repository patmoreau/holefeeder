import { Result } from './result';
import { combineWatchers } from './watch-utils';

jest.useFakeTimers();

describe('combineWatchers', () => {
  it('should emit combined value when all sources emit success', () => {
    const watcher1 = jest.fn((cb) => {
      cb(Result.success('A'));
      return jest.fn();
    });
    const watcher2 = jest.fn((cb) => {
      cb(Result.success(1));
      return jest.fn();
    });

    const combined = combineWatchers([watcher1, watcher2], (a, b) => `${a}-${b}`);
    const spy = jest.fn();

    const unsub = combined(spy);

    expect(spy).toHaveBeenLastCalledWith(expect.objectContaining({ isFailure: false, isLoading: false, value: 'A-1' }));
    unsub();
  });

  it('should wait for all sources to emit', () => {
    let cb1: any;
    const watcher1 = jest.fn((cb) => {
      cb1 = cb;
      return jest.fn();
    });
    const watcher2 = jest.fn((cb) => {
      cb(Result.success(2));
      return jest.fn();
    });

    const combined = combineWatchers([watcher1, watcher2], (a, b) => `${a}-${b}`);
    const spy = jest.fn();

    const unsub = combined(spy);

    // Initial state (watcher2 emitted, watcher1 hasn't)
    expect(spy).toHaveBeenCalledWith(expect.objectContaining({ isLoading: true }));

    // Now emit watcher1
    cb1(Result.success('B'));

    expect(spy).toHaveBeenLastCalledWith(expect.objectContaining({ isFailure: false, isLoading: false, value: 'B-2' }));
    unsub();
  });

  it('should emit failure if any source fails', () => {
    const watcher1 = jest.fn((cb) => {
      cb(Result.failure(['error1']));
      return jest.fn();
    });
    const watcher2 = jest.fn((cb) => {
      cb(Result.success(1));
      return jest.fn();
    });

    const combined = combineWatchers([watcher1, watcher2], (a, b) => `${a}-${b}`);
    const spy = jest.fn();

    combined(spy);

    expect(spy).toHaveBeenLastCalledWith(expect.objectContaining({ isFailure: true }));
    expect(spy.mock.calls[spy.mock.calls.length - 1][0].errors).toContain('error1');
  });

  it('should emit loading if any source is loading after init', () => {
    const watcher1 = jest.fn((cb) => {
      cb(Result.loading());
      return jest.fn();
    });
    const watcher2 = jest.fn((cb) => {
      cb(Result.success(1));
      return jest.fn();
    });

    const combined = combineWatchers([watcher1, watcher2], (a, b) => `${a}-${b}`);
    const spy = jest.fn();

    combined(spy);

    expect(spy).toHaveBeenLastCalledWith(expect.objectContaining({ isLoading: true }));
  });
});
