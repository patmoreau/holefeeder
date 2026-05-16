import type { PropsWithChildren } from 'react';
import type { StyleProp, ViewStyle } from 'react-native';

export type HorizontalScrollViewModuleEvents = {
  onChange: (params: ChangeEventPayload) => void;
};

export type ChangeEventPayload = {
  value: string;
};

export type HorizontalScrollViewProps = PropsWithChildren<{
  style?: StyleProp<ViewStyle>;
}>;
