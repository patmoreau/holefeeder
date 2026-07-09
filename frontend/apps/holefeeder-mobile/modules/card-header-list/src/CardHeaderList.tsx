import { requireNativeView } from 'expo';
import React from 'react';
import { CardHeaderListProps } from './CardHeaderList.types';

const NativeView = requireNativeView<CardHeaderListProps>('CardHeaderList');

export default function CardHeaderList({ children, style, contentInsetTop, onScrollOffsetChange }: CardHeaderListProps) {
  return (
    <NativeView style={style} contentInsetTop={contentInsetTop} onScrollOffsetChange={onScrollOffsetChange}>
      {children}
    </NativeView>
  );
}
