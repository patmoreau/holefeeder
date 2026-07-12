import { Menu, MenuProps } from '@expo/ui/swift-ui';

export type ExpoMenuProps = MenuProps & {};

export const ExpoMenu = (props: ExpoMenuProps) => {
  return <Menu {...props} />;
};
