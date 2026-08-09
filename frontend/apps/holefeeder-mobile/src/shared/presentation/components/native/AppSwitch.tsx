import { ExpoSwitch } from './expo/ExpoSwitch';

type Props = {
  value: boolean;
  onChange?: (value: boolean) => void;
  disabled?: boolean;
  testID?: string;
};

export const AppSwitch = ({ value, onChange = () => {}, disabled, testID }: Props) => {
  return <ExpoSwitch value={value} onValueChange={onChange} disabled={disabled} testID={testID} />;
};
