import { requireNativeView } from 'expo';
import React from 'react';
import { HorizontalScrollViewProps } from './HorizontalScrollView.types';

const NativeView = requireNativeView<HorizontalScrollViewProps>('HorizontalScrollView');

export default function HorizontalScrollView({ children, style }: HorizontalScrollViewProps) {
  return <NativeView style={style}>{children}</NativeView>;
}
