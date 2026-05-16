/* eslint-disable @typescript-eslint/no-require-imports */
const React = require('react');
const { View, Text: RNText, Button: RNButton, TextInput } = require('react-native');

export const Host = ({ children, style }: { children: unknown; style?: unknown }) =>
  React.createElement(View, { testID: 'host', style }, children);

export const BottomSheet = ({ children, isOpened }: { children: unknown; isOpened: boolean }) =>
  isOpened ? React.createElement(View, { testID: 'bottom-sheet' }, children) : null;

export const VStack = ({ children }: { children: unknown }) => React.createElement(View, { testID: 'vstack' }, children);

export const HStack = ({ children }: { children: unknown }) =>
  React.createElement(View, { testID: 'hstack', style: { flexDirection: 'row' } }, children);

export const Text = ({ children }: { children: unknown }) =>
  React.createElement(RNText, { testID: typeof children === 'string' ? children : 'text' }, children);

export const TextField = ({ children, placeholder }: { children: unknown; placeholder: unknown }) =>
  React.createElement(TextInput, { placeholder }, children);

export const Button = ({ children, variant, role, onPress }: { children: unknown; variant?: string; role?: string; onPress?: () => void }) =>
  React.createElement(
    View,
    { testID: 'button', 'data-variant': variant, 'data-role': role },
    React.createElement(RNButton, { title: typeof children === 'string' ? children : 'Button', onPress }),
    React.createElement(View, null, children)
  );

export const Image = ({ systemName, onPress }: { systemName: string; onPress: unknown }) =>
  React.createElement(View, { testID: `image-${systemName}`, onPress });
