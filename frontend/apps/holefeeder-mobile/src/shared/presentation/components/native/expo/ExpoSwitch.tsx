import { Switch, SwitchProps } from '@expo/ui';

export type ExpoSwitchProps = SwitchProps & {};

export const ExpoSwitch = ({ value, onValueChange, ...props }: ExpoSwitchProps) => {
  return <Switch value={value} onValueChange={onValueChange} {...props} />;
};
