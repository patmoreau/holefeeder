import { createModifier, createModifierWithEventListener } from '@expo/ui/swift-ui/modifiers';

// The callback has to travel under the `eventListener` key — that is what the native
// dispatcher invokes. Passing it under any other name leaves the modifier inert.
export const onScrollOffsetChange = (onChange: (offsetY: number) => void) =>
  createModifierWithEventListener('onScrollOffsetChange', (event: { offsetY: number }) => onChange(event.offsetY));

export type ScrollTransitionParams = {
  scaleIdentity?: number;
  scaleOther?: number;
  opacityIdentity?: number;
  opacityOther?: number;
};

export const scrollTransition = (params: ScrollTransitionParams = {}) => createModifier('scrollTransition', params);
