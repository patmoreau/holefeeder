import React from 'react';
import { StyleProp, View, ViewStyle } from 'react-native';

export const HorizontalScrollView = ({ children, style }: { children: React.ReactNode; style?: StyleProp<ViewStyle> }) => (
  <View testID="horizontal-scroll-view" style={style}>
    {children}
  </View>
);
