import {
  background,
  border,
  cornerRadius,
  foregroundStyle,
  frame,
  ignoreSafeArea,
  listRowBackground,
  listRowInsets,
  listRowSeparator,
  onAppear,
} from '@expo/ui/swift-ui/modifiers';
import type { ColorValue } from 'react-native';
import {
  collapsingHeaderBar,
  collapsingHeaderLargeCard,
  collapsingHeaderSmallCard,
  collapsingHeaderSource,
  onScrollOffsetChange,
} from '@/modules/app-modifiers';

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
  onAppear: (handler: () => void) => onAppear(handler),
  background: (color: Color) => background(color),
  border: (color: Color) => border({ color: color }),
  cornerRadius: (radius: number) => cornerRadius(radius),
  foregroundStyle: (color: Color) => foregroundStyle(color),
  onScrollOffsetChange: (handler: (offsetY: number) => void) => onScrollOffsetChange(handler),
  collapsingHeaderSource: (id: string) => collapsingHeaderSource(id),
  collapsingHeaderBar: (params: Parameters<typeof collapsingHeaderBar>[0]) => collapsingHeaderBar(params),
  collapsingHeaderLargeCard: (params: Parameters<typeof collapsingHeaderLargeCard>[0]) => collapsingHeaderLargeCard(params),
  collapsingHeaderSmallCard: (params: Parameters<typeof collapsingHeaderSmallCard>[0]) => collapsingHeaderSmallCard(params),
  listRowBackground: (color: Color) => listRowBackground(color),
  listRowInsets: (params: Parameters<typeof listRowInsets>[0]) => listRowInsets(params),
  hideListRowSeparator: listRowSeparator('hidden'),
  ignoreTopSafeArea: ignoreSafeArea({ regions: 'container', edges: 'top' }),
} as const;
