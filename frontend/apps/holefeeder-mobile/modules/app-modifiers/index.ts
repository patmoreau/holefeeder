import { createModifier } from '@expo/ui/swift-ui/modifiers';

export const onScrollOffsetChange = (onChange: (offsetY: number) => void) => createModifier('onScrollOffsetChange', { onChange });

export type ScrollTransitionParams = {
  scaleIdentity?: number;
  scaleOther?: number;
  opacityIdentity?: number;
  opacityOther?: number;
};

export const scrollTransition = (params: ScrollTransitionParams = {}) => createModifier('scrollTransition', params);
