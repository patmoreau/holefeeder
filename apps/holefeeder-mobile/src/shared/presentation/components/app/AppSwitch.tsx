import { ExpoSwitch } from '../expo/ExpoSwitch';

type Props = {
  value: boolean;
  onChange: (value: boolean) => void;
  readonly?: boolean;
};

export const AppSwitch = ({ value, onChange, readonly }: Props) => {
  return <ExpoSwitch value={value} onValueChange={onChange} disabled={readonly} />;
};
