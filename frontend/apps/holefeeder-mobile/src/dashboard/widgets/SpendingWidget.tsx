import { HStack, Image, Spacer, Text, VStack } from '@expo/ui/swift-ui';
import {
  containerBackground,
  font,
  foregroundStyle,
  frame,
  lineLimit,
  minimumScaleFactor,
  monospacedDigit,
  resizable,
  widgetURL,
} from '@expo/ui/swift-ui/modifiers';
import { createWidget } from 'expo-widgets';
import { PlatformColor } from 'react-native';

export type SpendingTrendProps = {
  text: string;
  direction: 'down' | 'up';
};

export type SpendingProjectedProps = {
  label: string;
  text: string;
  tone: 'positive' | 'negative' | 'neutral';
};

export type SpendingWidgetProps =
  | { state: 'signed-out'; logo?: string; message: string }
  | { state: 'ready'; logo?: string; title: string; spending: string; trend?: SpendingTrendProps; projected: SpendingProjectedProps };

const SpendingWidget = (props: SpendingWidgetProps) => {
  'widget';
  const background = containerBackground(PlatformColor('systemBackground'), 'widget');
  const link = widgetURL('holefeeder://');
  const fill = frame({ maxWidth: 10000, maxHeight: 10000, alignment: 'topLeading' });
  const secondary = PlatformColor('secondaryLabel');
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

  const toneColor = { positive: PlatformColor('systemGreen'), negative: PlatformColor('systemRed'), neutral: PlatformColor('label') };
  const caption = (text: string) => (
    <Text modifiers={[font({ textStyle: 'caption' }), foregroundStyle(secondary), lineLimit(1), minimumScaleFactor(0.7)]}>{text}</Text>
  );

  return (
    <VStack alignment="leading" spacing={2} modifiers={[background, link, fill]}>
      <HStack alignment="top">
        {caption(props.title)}
        <Spacer />
        {tile(32)}
      </HStack>
      <Text modifiers={[font({ size: 26, weight: 'bold', design: 'rounded' }), monospacedDigit(), lineLimit(1), minimumScaleFactor(0.5)]}>
        {props.spending}
      </Text>
      {props.trend ? (
        <HStack spacing={3}>
          <Image
            systemName={props.trend.direction === 'up' ? 'arrow.up.right' : 'arrow.down.right'}
            modifiers={[
              font({ textStyle: 'caption', weight: 'bold' }),
              foregroundStyle(props.trend.direction === 'up' ? toneColor.negative : toneColor.positive),
            ]}
          />
          <Text
            modifiers={[
              font({ textStyle: 'caption', weight: 'semibold' }),
              foregroundStyle(props.trend.direction === 'up' ? toneColor.negative : toneColor.positive),
              lineLimit(1),
              minimumScaleFactor(0.7),
            ]}
          >
            {props.trend.text}
          </Text>
        </HStack>
      ) : null}
      <Spacer />
      {caption(props.projected.label)}
      <Text
        modifiers={[
          font({ size: 17, weight: 'semibold' }),
          monospacedDigit(),
          foregroundStyle(toneColor[props.projected.tone]),
          lineLimit(1),
          minimumScaleFactor(0.6),
        ]}
      >
        {props.projected.text}
      </Text>
    </VStack>
  );
};

export default createWidget('SpendingWidget', SpendingWidget, { state: 'signed-out', message: 'Open Holefeeder to see your spending' });
