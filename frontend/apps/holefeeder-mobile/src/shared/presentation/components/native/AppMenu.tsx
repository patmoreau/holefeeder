import { ExpoMenu, ExpoMenuProps } from './expo/ExpoMenu';

type Props = ExpoMenuProps & {};

export const AppMenu = (props: Props) => {
  return <ExpoMenu {...props} />;
};
