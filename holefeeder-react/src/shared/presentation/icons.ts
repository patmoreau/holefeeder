import { MaterialIcons } from '@expo/vector-icons';
import type { SymbolViewProps } from 'expo-symbols';
import { ComponentProps } from 'react';

type IconMapping = Record<Extract<SymbolViewProps['name'], string>, ComponentProps<typeof MaterialIcons>['name']>;

export const AppIcons = {
  account: 'creditcard',
  accounts: 'wallet.bifold.fill',
  add: 'plus',
  back: 'chevron.backward',
  calendar: 'calendar',
  cashflow: 'chart.line.uptrend.xyaxis',
  category: 'tray.2',
  close: 'xmark',
  connected: 'rectangle.connected.to.line.below',
  dashboard: 'rectangle.3.group.fill',
  description: 'pencil.and.list.clipboard',
  edit: 'square.and.pencil',
  download: 'square.and.arrow.down',
  expand: 'chevron.right',
  expiresAt: 'arrow.trianglehead.2.clockwise',
  frequency: 'clock.badge.exclamationmark',
  language: 'globe',
  purchase: 'cart',
  uploadOutstanding: 'square.and.arrow.up.trianglebadge.exclamationmark',
  save: 'plus.circle.fill',
  settings: 'gearshape.fill',
  share: 'square.and.arrow.up',
  storeItem: 'shippingbox',
  sync: 'arrow.trianglehead.2.clockwise',
  tag: 'tag',
  theme: 'pencil.and.scribble',
  token: 'key.horizontal',
  trendUp: 'chart.line.uptrend.xyaxis',
  trendDown: 'chart.line.downtrend.xyaxis',
  upload: 'square.and.arrow.up',
  warning: 'exclamationmark.triangle',
} as const;

export type AppIcons = (typeof AppIcons)[keyof typeof AppIcons];

/**
 * Add your SF Symbols to Material Icons mappings here.
 * - see Material Icons in the [Icons Directory](https://icons.expo.fyi).
 * - see SF Symbols in the [SF Symbols](https://developer.apple.com/sf-symbols/) app.
 */
export const AppIconsMapping = {
  'arrow.trianglehead.2.clockwise': 'refresh',
  calendar: 'calendar-today',
  cart: 'shopping-cart',
  'chart.bar.xaxis': 'bar-chart',
  'chart.line.downtrend.xyaxis': 'trending-down',
  'chart.line.uptrend.xyaxis': 'trending-up',
  'chevron.backward': 'chevron-left',
  'chevron.right': 'chevron-right',
  xmark: 'close',
  'clock.badge.exclamationmark': 'lock-clock',
  creditcard: 'credit-card',
  'exclamationmark.triangle': 'warning',
  'gearshape.fill': 'settings',
  globe: 'language',
  'key.horizontal': 'key',
  'pencil.and.list.clipboard': 'edit-note',
  'square.and.pencil': 'edit',
  'pencil.and.scribble': 'edit',
  plus: 'add',
  'plus.circle.fill': 'save',
  'rectangle.3.group.fill': 'dashboard',
  'rectangle.connected.to.line.below': 'network-check',
  shippingbox: 'storefront',
  'square.and.arrow.down': 'download',
  'square.and.arrow.up': 'upload',
  'square.and.arrow.up.trianglebadge.exclamationmark': 'file-upload-off',
  tag: 'label',
  'tray.2': 'all-inbox',
  'wallet.bifold.fill': 'wallet',
} as const satisfies Partial<IconMapping>;
