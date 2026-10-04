import { act, renderHook } from '@testing-library/react-native';
import { Keyboard, KeyboardEvent } from 'react-native';
import { useKeyboardHeight } from '@/shared/presentation/core/use-keyboard-height';

type Listener = (event: KeyboardEvent) => void;

describe('useKeyboardHeight', () => {
  let listeners: Record<string, Listener>;
  let removed: string[];

  beforeEach(() => {
    listeners = {};
    removed = [];
    jest.spyOn(Keyboard, 'addListener').mockImplementation(((name: string, listener: Listener) => {
      listeners[name] = listener;
      return { remove: () => removed.push(name) };
    }) as unknown as typeof Keyboard.addListener);
  });

  afterEach(() => jest.restoreAllMocks());

  const keyboardEvent = (height: number) => ({ endCoordinates: { height: height, screenX: 0, screenY: 0, width: 402 } }) as KeyboardEvent;

  it('starts at zero', async () => {
    const { result } = await renderHook(() => useKeyboardHeight());

    expect(result.current).toBe(0);
  });

  it('follows the keyboard height when it shows', async () => {
    const { result } = await renderHook(() => useKeyboardHeight());

    await act(async () => listeners.keyboardWillShow(keyboardEvent(291)));

    expect(result.current).toBe(291);
  });

  it('returns to zero when the keyboard hides', async () => {
    const { result } = await renderHook(() => useKeyboardHeight());
    await act(async () => listeners.keyboardWillShow(keyboardEvent(291)));

    await act(async () => listeners.keyboardWillHide(keyboardEvent(0)));

    expect(result.current).toBe(0);
  });

  it('stops listening when unmounted', async () => {
    const { unmount } = await renderHook(() => useKeyboardHeight());

    await act(async () => unmount());

    expect(removed.sort()).toEqual(['keyboardWillHide', 'keyboardWillShow']);
  });
});
