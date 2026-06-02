import Add from '@expo/material-symbols/add.xml';
import AddCircle from '@expo/material-symbols/add_circle.xml';
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

export type UniversalIcon = {
  ios: Extract<SymbolViewProps['name'], string>;
  android: ImageSourcePropType;
};

const universalIconMapping = {
  account: { ios: 'creditcard', android: CreditCard },
  accounts: { ios: 'wallet.bifold.fill', android: Wallet },
  add: { ios: 'plus', android: Add },
  back: { ios: 'chevron.backward', android: ChevronLeft },
  calendar: { ios: 'calendar', android: CalendarToday },
  cashflow: { ios: 'chart.line.uptrend.xyaxis', android: TrendingUp },
  category: { ios: 'tray.2', android: Label },
  close: { ios: 'xmark', android: Close },
  connected: {
    ios: 'rectangle.connected.to.line.below',
    android: NetworkCheck,
  },
  dashboard: { ios: 'rectangle.3.group.fill', android: Dashboard },
  delete: { ios: 'delete.left', android: Delete },
  description: { ios: 'pencil.and.list.clipboard', android: EditNote },
  edit: { ios: 'square.and.pencil', android: Edit },
  download: { ios: 'square.and.arrow.down', android: Download },
  expand: { ios: 'chevron.right', android: ChevronRight },
  expiresAt: { ios: 'arrow.trianglehead.2.clockwise', android: LockClock },
  frequency: { ios: 'clock.badge.exclamationmark', android: FileUploadOff },
  language: { ios: 'globe', android: Language },
  purchase: { ios: 'cart', android: ShoppingCart },
  uploadOutstanding: {
    ios: 'square.and.arrow.up.trianglebadge.exclamationmark',
    android: FileUploadOff,
  },
  save: { ios: 'plus.circle.fill', android: AddCircle },
  settings: { ios: 'gearshape.fill', android: Settings },
  share: { ios: 'square.and.arrow.up', android: Upload },
  storeItem: { ios: 'shippingbox', android: Storefront },
  sync: { ios: 'arrow.trianglehead.2.clockwise', android: Sync },
  tag: { ios: 'tag', android: Label },
  theme: { ios: 'pencil.and.scribble', android: Edit },
  token: { ios: 'key.horizontal', android: Key },
  trendUp: { ios: 'chart.line.uptrend.xyaxis', android: TrendingUp },
  trendDown: { ios: 'chart.line.downtrend.xyaxis', android: TrendingDown },
  upload: { ios: 'square.and.arrow.up', android: Upload },
  warning: { ios: 'exclamationmark.triangle', android: Warning },
} satisfies Record<string, UniversalIcon>;

export type AppIconName = keyof typeof universalIconMapping;

export const AppIconMap = universalIconMapping;
