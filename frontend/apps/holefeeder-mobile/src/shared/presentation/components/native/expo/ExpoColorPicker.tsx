import { ColorPicker, ColorPickerProps } from '@expo/ui/swift-ui';

export type ExpoColorPickerProps = ColorPickerProps & {};

export const ExpoColorPicker = ({ selection, onSelectionChange, ...props }: ExpoColorPickerProps) => {
  return <ColorPicker selection={selection} onSelectionChange={onSelectionChange} {...props} />;
};
