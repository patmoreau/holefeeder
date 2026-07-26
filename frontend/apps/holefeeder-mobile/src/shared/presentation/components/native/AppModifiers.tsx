import { background, border, cornerRadius, foregroundStyle, frame } from '@expo/ui/swift-ui/modifiers';
import type { ColorValue } from 'react-native';

type NamedColor =
  | 'primary'
  | 'secondary'
  | 'red'
  | 'orange'
  | 'yellow'
  | 'green'
  | 'blue'
  | 'purple'
  | 'pink'
  | 'white'
  | 'gray'
  | 'black'
  | 'clear'
  | 'mint'
  | 'teal'
  | 'cyan'
  | 'indigo'
  | 'brown';

export type Color = string | ColorValue | NamedColor;

export const AppModifiers = {
  fillWidth: frame({ maxWidth: Infinity }),
  fillMaxSize: frame({ maxWidth: Infinity, maxHeight: Infinity }),
  frame: (params: Parameters<typeof frame>[0]) => frame(params),
  background: (color: Color) => background(color),
  border: (color: Color) => border({ color: color }),
  cornerRadius: (radius: number) => cornerRadius(radius),
  foregroundStyle: (color: Color) => foregroundStyle(color),
} as const;
