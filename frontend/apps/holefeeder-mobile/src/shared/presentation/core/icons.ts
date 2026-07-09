import Add from '@expo/material-symbols/add.xml';
import AddCircle from '@expo/material-symbols/add_circle.xml';
import AllInbox from '@expo/material-symbols/all_inbox.xml';
import CalendarToday from '@expo/material-symbols/calendar_today.xml';
import ChevronLeft from '@expo/material-symbols/chevron_left.xml';
import ChevronRight from '@expo/material-symbols/chevron_right.xml';
import Close from '@expo/material-symbols/close.xml';
import CreditCard from '@expo/material-symbols/credit_card.xml';
import Dashboard from '@expo/material-symbols/dashboard.xml';
import Delete from '@expo/material-symbols/delete.xml';
import Download from '@expo/material-symbols/download.xml';
import Edit from '@expo/material-symbols/edit.xml';
import EditNote from '@expo/material-symbols/edit_note.xml';
import FileUploadOff from '@expo/material-symbols/file_upload_off.xml';
import Key from '@expo/material-symbols/key.xml';
import Label from '@expo/material-symbols/label.xml';
import Language from '@expo/material-symbols/language.xml';
import LockClock from '@expo/material-symbols/lock_clock.xml';
import NetworkCheck from '@expo/material-symbols/network_check.xml';
import Settings from '@expo/material-symbols/settings.xml';
import ShoppingCart from '@expo/material-symbols/shopping_cart.xml';
import Storefront from '@expo/material-symbols/storefront.xml';
import Sync from '@expo/material-symbols/sync.xml';
import TrendingDown from '@expo/material-symbols/trending_down.xml';
import TrendingUp from '@expo/material-symbols/trending_up.xml';
import Upload from '@expo/material-symbols/upload.xml';
import Wallet from '@expo/material-symbols/wallet.xml';
import Warning from '@expo/material-symbols/warning.xml';
import type { SymbolViewProps } from 'expo-symbols';
import { ImageSourcePropType } from 'react-native';

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
  delete: 'delete',
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

export type AppIconsMappings = {
  ios: Extract<SymbolViewProps['name'], string>;
  android: ImageSourcePropType;
};

export const AppIconsSelect: Record<string, AppIconsMappings> = {
  add: { ios: 'plus', android: Add },
};

export const AppIconsMaterialMapping: Record<AppIcons, ImageSourcePropType> = {
  'arrow.trianglehead.2.clockwise': Sync,
  calendar: CalendarToday,
  cart: ShoppingCart,
  'chart.line.downtrend.xyaxis': TrendingDown,
  'chart.line.uptrend.xyaxis': TrendingUp,
  'chevron.backward': ChevronLeft,
  'chevron.right': ChevronRight,
  'clock.badge.exclamationmark': LockClock,
  creditcard: CreditCard,
  delete: Delete,
  'exclamationmark.triangle': Warning,
  'gearshape.fill': Settings,
  globe: Language,
  'key.horizontal': Key,
  'pencil.and.list.clipboard': EditNote,
  'pencil.and.scribble': Edit,
  plus: Add,
  'plus.circle.fill': AddCircle,
  'rectangle.3.group.fill': Dashboard,
  'rectangle.connected.to.line.below': NetworkCheck,
  shippingbox: Storefront,
  'square.and.arrow.down': Download,
  'square.and.arrow.up': Upload,
  'square.and.arrow.up.trianglebadge.exclamationmark': FileUploadOff,
  'square.and.pencil': Edit,
  tag: Label,
  'tray.2': AllInbox,
  'wallet.bifold.fill': Wallet,
  xmark: Close,
};
