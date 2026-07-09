import React from 'react';
import { StyleProp, View, ViewStyle } from 'react-native';

export const CardHeaderList = ({
  children,
  style,
}: {
  children: React.ReactNode;
  style?: StyleProp<ViewStyle>;
  contentInsetTop?: number;
  onScrollOffsetChange?: (event: { offset: number }) => void;
}) => (
  <View testID="card-header-list" style={style}>
    {children}
  </View>
);
