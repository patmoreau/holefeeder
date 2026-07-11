import { ExpoColorPicker } from './expo/ExpoColorPicker';

type Props = {
  value: string;
  onChange?: (value: string) => void;
  label?: string;
  supportsOpacity?: boolean;
};

export const AppColorPicker = ({ value, onChange = () => {}, label, supportsOpacity = false }: Props) => {
  return <ExpoColorPicker selection={value} onSelectionChange={onChange} label={label} supportsOpacity={supportsOpacity} />;
};
