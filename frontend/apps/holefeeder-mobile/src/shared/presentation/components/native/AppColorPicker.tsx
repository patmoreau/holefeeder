import { ExpoColorPicker } from './expo/ExpoColorPicker';

type Props = {
  value: string;
  onChange?: (value: string) => void;
  label?: string;
  supportsOpacity?: boolean;
  testID?: string;
};

export const AppColorPicker = ({ value, onChange = () => {}, label, supportsOpacity = false, testID }: Props) => {
  return <ExpoColorPicker selection={value} onSelectionChange={onChange} label={label} supportsOpacity={supportsOpacity} testID={testID} />;
};
