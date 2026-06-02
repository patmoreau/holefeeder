import type { PropsWithChildren } from 'react';
import type { StyleProp, ViewStyle } from 'react-native';

export type ScrollOffsetChangeEventPayload = {
  offset: number;
};

export type CardHeaderListProps = PropsWithChildren<{
  style?: StyleProp<ViewStyle>;
  contentInsetTop?: number;
  onScrollOffsetChange?: (event: ScrollOffsetChangeEventPayload) => void;
}>;
