import { ExpoSwitch } from './expo/ExpoSwitch';

type Props = {
  value: boolean;
  onChange?: (value: boolean) => void;
  disabled?: boolean;
};

export const AppSwitch = ({ value, onChange = () => {}, disabled }: Props) => {
  return <ExpoSwitch value={value} onValueChange={onChange} disabled={disabled} />;
};
