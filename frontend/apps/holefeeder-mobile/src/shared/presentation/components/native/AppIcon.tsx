import { Icon } from '@expo/ui';
import type { UniversalIcon } from '@/shared/presentation/core/app-icon-map';
import { ExpoIcon, ExpoIconProps } from './expo/ExpoIcon';

export type AppIconProps = ExpoIconProps & {};

const AppIconComponent = (props: AppIconProps) => {
  return <ExpoIcon {...props} />;
};

// Resolves a UniversalIcon to the platform-native icon value for props that
// expect a name/source rather than a component (e.g. Stack.Toolbar buttons).
export const AppIcon = Object.assign(AppIconComponent, {
  select: (icon: UniversalIcon) => Icon.select(icon),
});
