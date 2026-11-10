import { HStack, Spacer, Text, VStack } from '@expo/ui/swift-ui';
import { containerBackground, font, foregroundStyle, frame, minimumScaleFactor, monospacedDigit, widgetURL } from '@expo/ui/swift-ui/modifiers';
import { createWidget, type WidgetEnvironment } from 'expo-widgets';
import { PlatformColor } from 'react-native';

export type BillsDueTotalProps = {
  count: number;
  amount: string;
  label: string;
};

export type BillsDueWidgetProps =
  { state: 'signed-out'; message: string } | { state: 'ready'; pastDue: BillsDueTotalProps; dueToday: BillsDueTotalProps };

const BillsDueWidget = (props: BillsDueWidgetProps, environment: WidgetEnvironment) => {
  'widget';
  const background = containerBackground(PlatformColor('systemBackground'), 'widget');
  const link = widgetURL('holefeeder://');

  if (props.state === 'signed-out') {
    return (
      <VStack
        alignment="leading"
        spacing={4}
        modifiers={[background, link, frame({ maxWidth: 10000, maxHeight: 10000, alignment: 'topLeading' })]}
      >
        <Text modifiers={[font({ textStyle: 'headline' }), foregroundStyle('#FF8D28')]}>Holefeeder</Text>
        <Text modifiers={[font({ textStyle: 'footnote' }), foregroundStyle(PlatformColor('secondaryLabel'))]}>{props.message}</Text>
      </VStack>
    );
  }

  const isMedium = environment.widgetFamily === 'systemMedium';
  const total = (value: BillsDueTotalProps, color: string | ReturnType<typeof PlatformColor>) => (
    <VStack alignment="leading" spacing={0}>
      <Text modifiers={[font({ size: isMedium ? 40 : 34, weight: 'bold', design: 'rounded' }), monospacedDigit(), foregroundStyle(color)]}>
        {value.count}
      </Text>
      <Text modifiers={[font({ textStyle: 'caption' }), foregroundStyle(PlatformColor('secondaryLabel'))]}>{value.label}</Text>
      {isMedium && (
        <Text modifiers={[font({ textStyle: 'footnote', weight: 'semibold' }), monospacedDigit(), minimumScaleFactor(0.6)]}>
          {value.amount}
        </Text>
      )}
    </VStack>
  );
  const pastDueColor = props.pastDue.count > 0 ? PlatformColor('systemRed') : PlatformColor('label');

  if (isMedium) {
    return (
      <HStack alignment="top" modifiers={[background, link]}>
        {total(props.pastDue, pastDueColor)}
        <Spacer />
        {total(props.dueToday, '#FF8D28')}
        <Spacer />
      </HStack>
    );
  }

  return (
    <VStack alignment="leading" modifiers={[background, link, frame({ maxWidth: 10000, maxHeight: 10000, alignment: 'leading' })]}>
      {total(props.pastDue, pastDueColor)}
      <Spacer />
      {total(props.dueToday, '#FF8D28')}
    </VStack>
  );
};

export default createWidget('BillsDueWidget', BillsDueWidget, { state: 'signed-out', message: 'Open Holefeeder to see your bills' });
