import { Divider, HStack, Image, Spacer, Text, VStack } from '@expo/ui/swift-ui';
import {
  containerBackground,
  font,
  foregroundStyle,
  frame,
  minimumScaleFactor,
  monospacedDigit,
  resizable,
  widgetURL,
} from '@expo/ui/swift-ui/modifiers';
import { createWidget, type WidgetEnvironment } from 'expo-widgets';
import { PlatformColor } from 'react-native';

export type BillsDueTotalProps = {
  count: number;
  amount: string;
  label: string;
};

export type BillsDueWidgetProps =
  | { state: 'signed-out'; logo?: string; message: string }
  | { state: 'ready'; logo?: string; title: string; pastDue: BillsDueTotalProps; dueToday: BillsDueTotalProps };

const BillsDueWidget = (props: BillsDueWidgetProps, environment: WidgetEnvironment) => {
  'widget';
  const background = containerBackground(PlatformColor('systemBackground'), 'widget');
  const link = widgetURL('holefeeder://');
  const fill = frame({ maxWidth: 10000, maxHeight: 10000, alignment: 'topLeading' });
  const secondary = PlatformColor('secondaryLabel');
  const orange = '#FF8D28';
  const tile = (size: number) =>
    props.logo ? <Image uiImage={props.logo} modifiers={[resizable(), frame({ width: size, height: size })]} /> : null;

  if (props.state === 'signed-out') {
    return (
      <VStack alignment="leading" spacing={8} modifiers={[background, link, fill]}>
        {tile(44)}
        <Spacer />
        <Text modifiers={[font({ textStyle: 'headline' })]}>Holefeeder</Text>
        <Text modifiers={[font({ textStyle: 'caption' }), foregroundStyle(secondary)]}>{props.message}</Text>
      </VStack>
    );
  }

  const pastDueColor = props.pastDue.count > 0 ? PlatformColor('systemRed') : PlatformColor('label');
  const count = (value: BillsDueTotalProps, color: string | ReturnType<typeof PlatformColor>, size: number) => (
    <Text modifiers={[font({ size: size, weight: 'bold', design: 'rounded' }), monospacedDigit(), foregroundStyle(color)]}>{value.count}</Text>
  );
  const label = (value: BillsDueTotalProps) => (
    <Text modifiers={[font({ textStyle: 'caption' }), foregroundStyle(secondary)]}>{value.label}</Text>
  );

  if (environment.widgetFamily === 'systemMedium') {
    const column = (value: BillsDueTotalProps, color: string | ReturnType<typeof PlatformColor>) => (
      <VStack alignment="leading" spacing={2} modifiers={[frame({ maxWidth: 10000, alignment: 'leading' })]}>
        {count(value, color, 40)}
        {label(value)}
        <Text modifiers={[font({ textStyle: 'subheadline', weight: 'semibold' }), monospacedDigit(), minimumScaleFactor(0.6)]}>
          {value.amount}
        </Text>
      </VStack>
    );

    return (
      <VStack alignment="leading" spacing={10} modifiers={[background, link, fill]}>
        <HStack spacing={8}>
          {tile(24)}
          <Text modifiers={[font({ textStyle: 'subheadline', weight: 'semibold' })]}>{props.title}</Text>
        </HStack>
        <HStack alignment="top" spacing={16}>
          {column(props.pastDue, pastDueColor)}
          <Divider />
          {column(props.dueToday, orange)}
        </HStack>
      </VStack>
    );
  }

  return (
    <HStack alignment="top" modifiers={[background, link, fill]}>
      <VStack alignment="leading" spacing={0}>
        {count(props.pastDue, pastDueColor, 34)}
        {label(props.pastDue)}
        <Spacer />
        {count(props.dueToday, orange, 34)}
        {label(props.dueToday)}
      </VStack>
      <Spacer />
      {tile(32)}
    </HStack>
  );
};

export default createWidget('BillsDueWidget', BillsDueWidget, { state: 'signed-out', message: 'Open Holefeeder to see your bills' });
