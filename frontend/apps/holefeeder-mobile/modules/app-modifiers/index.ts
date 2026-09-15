import { createModifier, createModifierWithEventListener } from '@expo/ui/swift-ui/modifiers';

// The callback has to travel under the `eventListener` key — that is what the native
// dispatcher invokes. Passing it under any other name leaves the modifier inert.
export const onScrollOffsetChange = (onChange: (offsetY: number) => void) =>
  createModifierWithEventListener('onScrollOffsetChange', (event: { offsetY: number }) => onChange(event.offsetY));

export type CollapsingHeaderBarParams = {
  id: string;
  fullHeight: number;
  collapsedHeight: number;
  backgroundColor: string;
};

export type CollapsingHeaderLargeCardParams = {
  id: string;
  travel: number;
  fadeEnd: number;
};

export type CollapsingHeaderSmallCardParams = {
  id: string;
  fullHeight: number;
  collapsedHeight: number;
  rowInset: number;
  rowHeight: number;
  fadeStart: number;
  fadeEnd: number;
};

export const collapsingHeaderSource = (id: string) => createModifier('collapsingHeaderSource', { id });

export const collapsingHeaderBar = (params: CollapsingHeaderBarParams) => createModifier('collapsingHeaderBar', params);

export const collapsingHeaderLargeCard = (params: CollapsingHeaderLargeCardParams) => createModifier('collapsingHeaderLargeCard', params);

export const collapsingHeaderSmallCard = (params: CollapsingHeaderSmallCardParams) => createModifier('collapsingHeaderSmallCard', params);

export type ScrollTransitionParams = {
  scaleIdentity?: number;
  scaleOther?: number;
  opacityIdentity?: number;
  opacityOther?: number;
};

export const scrollTransition = (params: ScrollTransitionParams = {}) => createModifier('scrollTransition', params);
