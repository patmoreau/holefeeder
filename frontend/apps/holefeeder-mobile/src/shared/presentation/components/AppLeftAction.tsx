import React from 'react';
import { RectButton } from 'react-native-gesture-handler';
import { SwipeableMethods } from 'react-native-gesture-handler/lib/typescript/components/ReanimatedSwipeable';
import Animated, { Extrapolation, interpolate, SharedValue, useAnimatedStyle } from 'react-native-reanimated';
import { useStyles } from '@/shared/theme/core/use-styles';

const createStyles = () => ({
  leftAction: {
    flex: 1,
    backgroundColor: '#497AFC' as const,
    justifyContent: 'center' as const,
  },
  archiveText: {
    color: 'white' as const,
    fontSize: 16,
    backgroundColor: 'transparent' as const,
    padding: 20,
  },
  actionText: {
    color: 'white' as const,
    fontSize: 16,
    backgroundColor: 'transparent' as const,
    textAlign: 'center' as const,
    margin: 'auto' as const,
  },
  leftActionView: {
    flex: 1,
  },
});

type LeftActionsProps = {
  text: string;
  dragX: SharedValue<number>;
  swipeableRef: React.RefObject<SwipeableMethods | null>;
  onAction?: () => void;
};

export const AppLeftAction = ({ text, dragX, swipeableRef, onAction }: LeftActionsProps) => {
  const styles = useStyles(createStyles);
  const animatedStyle = useAnimatedStyle(() => ({
    transform: [
      {
        translateX: interpolate(dragX.value, [0, 50, 100, 101], [-20, 0, 0, 1], Extrapolation.CLAMP),
      },
    ],
  }));

  return (
    <RectButton
      style={styles.leftAction}
      onPress={() => {
        onAction?.();
        swipeableRef.current!.close();
      }}
    >
      <Animated.Text style={[styles.archiveText, animatedStyle]}>{text}</Animated.Text>
    </RectButton>
  );
};
