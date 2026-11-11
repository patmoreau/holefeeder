import { Divider, HStack, Image, Spacer, Text, VStack } from '@expo/ui/swift-ui';
import {
  containerBackground,
  font,
  foregroundStyle,
  frame,
  hidden,
  lineLimit,
  minimumScaleFactor,
  monospacedDigit,
  resizable,
  widgetURL,
} from '@expo/ui/swift-ui/modifiers';
import { createWidget, type WidgetEnvironment } from 'expo-widgets';
import { PlatformColor } from 'react-native';

export type BillsDueAmountProps = {
  direction: 'down' | 'up' | 'none';
  text: string;
};

export type BillsDueTotalProps = {
  count: number;
  label: string;
  amounts: BillsDueAmountProps[];
};

export type BillsDueFigureProps = {
  label: string;
  text: string;
};

export type BillsDueProjectedProps = BillsDueFigureProps & {
  tone: 'positive' | 'negative' | 'neutral';
};

export type BillsDueWidgetProps =
  | { state: 'signed-out'; logo?: string; message: string }
  | {
      state: 'ready';
      logo?: string;
      title: string;
      pastDue: BillsDueTotalProps;
      dueToday: BillsDueTotalProps;
      spending: BillsDueFigureProps;
      projected: BillsDueProjectedProps;
    };

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
    <Text modifiers={[font({ textStyle: 'caption' }), foregroundStyle(secondary), lineLimit(1), minimumScaleFactor(0.7)]}>{value.label}</Text>
  );

  if (environment.widgetFamily === 'systemMedium') {
    const column = (value: BillsDueTotalProps, color: string | ReturnType<typeof PlatformColor>) => (
      <VStack alignment="leading" spacing={2} modifiers={[frame({ maxWidth: 10000, alignment: 'leading' })]}>
        <HStack spacing={10}>
          <Text modifiers={[font({ size: 40, weight: 'bold', design: 'rounded' }), monospacedDigit(), hidden(), frame({ height: 0 })]}>
            {value.count}
          </Text>
          {label(value)}
        </HStack>
        <HStack spacing={10}>
          {count(value, color, 40)}
          <VStack alignment="trailing" spacing={6}>
            {value.amounts.map((amount) => (
              <HStack key={amount.direction} spacing={3}>
                <Text
                  modifiers={[font({ textStyle: 'subheadline', weight: 'semibold' }), monospacedDigit(), lineLimit(1), minimumScaleFactor(0.6)]}
                >
                  {amount.text}
                </Text>
                {amount.direction !== 'none' ? (
                  <Image
                    systemName={amount.direction === 'down' ? 'arrow.down' : 'arrow.up'}
                    modifiers={[font({ textStyle: 'caption', weight: 'bold' }), foregroundStyle(secondary)]}
                  />
                ) : null}
              </HStack>
            ))}
          </VStack>
        </HStack>
      </VStack>
    );

    const toneColor = { positive: PlatformColor('systemGreen'), negative: PlatformColor('systemRed'), neutral: PlatformColor('label') };
    const figure = (value: BillsDueFigureProps, color: ReturnType<typeof PlatformColor>) => (
      <HStack alignment="firstTextBaseline" spacing={6} modifiers={[frame({ maxWidth: 10000, alignment: 'leading' })]}>
        <Text modifiers={[font({ textStyle: 'caption' }), foregroundStyle(secondary), lineLimit(1), minimumScaleFactor(0.7)]}>
          {value.label}
        </Text>
        <Text
          modifiers={[
            font({ textStyle: 'subheadline', weight: 'semibold' }),
            monospacedDigit(),
            foregroundStyle(color),
            lineLimit(1),
            minimumScaleFactor(0.6),
          ]}
        >
          {value.text}
        </Text>
      </HStack>
    );

    return (
      <VStack
        alignment="leading"
        spacing={8}
        modifiers={[background, link, frame({ maxWidth: 10000, maxHeight: 10000, alignment: 'leading' })]}
      >
        <HStack spacing={8}>
          {tile(24)}
          <Text modifiers={[font({ textStyle: 'subheadline', weight: 'semibold' })]}>{props.title}</Text>
        </HStack>
        <HStack alignment="center" spacing={16}>
          {column(props.pastDue, pastDueColor)}
          <Divider />
          {column(props.dueToday, orange)}
        </HStack>
        <Divider />
        <HStack alignment="top" spacing={33}>
          {figure(props.spending, PlatformColor('label'))}
          {figure(props.projected, toneColor[props.projected.tone])}
        </HStack>
      </VStack>
    );
  }

  return (
    <VStack alignment="leading" spacing={0} modifiers={[background, link, fill]}>
      <HStack alignment="top">
        {count(props.pastDue, pastDueColor, 34)}
        <Spacer />
        {tile(32)}
      </HStack>
      {label(props.pastDue)}
      <Spacer />
      {count(props.dueToday, orange, 34)}
      {label(props.dueToday)}
    </VStack>
  );
};

export default createWidget('BillsDueWidget', BillsDueWidget, { state: 'signed-out', message: 'Open Holefeeder to see your bills' });
