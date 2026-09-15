import { Row } from '@expo/ui';
import { contentShape, onTapGesture, shapes } from '@expo/ui/swift-ui/modifiers';
import { ExpoRowProps } from './ExpoRow';

export const ExpoRow = ({ onPress, modifiers = [], ...props }: ExpoRowProps) => {
  const allModifiers = onPress ? [...modifiers, contentShape(shapes.rectangle()), onTapGesture(onPress)] : modifiers;
  return <Row {...props} modifiers={allModifiers} />;
};
