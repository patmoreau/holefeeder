import React from 'react';
import { RectButton } from 'react-native-gesture-handler';
import { SwipeableMethods } from 'react-native-gesture-handler/lib/typescript/components/ReanimatedSwipeable';
import Animated, { interpolate, SharedValue, useAnimatedStyle } from 'react-native-reanimated';
import { AppText } from '@/shared/presentation/components/AppText';
import { useStyles } from '@/shared/theme/core/use-styles';

const createStyles = () => ({
  actionText: {
    color: 'white' as const,
    backgroundColor: 'transparent' as const,
    textAlign: 'center' as const,
    margin: 'auto' as const,
  },
  rightActionView: {
    flex: 1,
  },
  rightAction: {
    alignItems: 'center' as const,
    flex: 1,
    justifyContent: 'center' as const,
  },
});

type RightActionProps = {
  text: string;
  color: string;
  x: number;
  progress: SharedValue<number>;
  totalWidth: number;
  swipeableRef: React.RefObject<SwipeableMethods | null>;
  onAction?: () => void;
};

export const AppRightAction = ({ text, color, x, progress, totalWidth, swipeableRef, onAction }: RightActionProps) => {
  const styles = useStyles(createStyles);
  const animatedStyle = useAnimatedStyle(() => ({
    transform: [
      {
        translateX: interpolate(progress.value, [0, -totalWidth], [x, 0]),
      },
    ],
  }));

  const pressHandler = () => {
    onAction?.();
    swipeableRef.current?.close();
  };

  return (
    <Animated.View style={[styles.rightActionView, animatedStyle]}>
      <RectButton style={[styles.rightAction, { backgroundColor: color }]} onPress={pressHandler}>
        <AppText variant={'default'} style={styles.actionText}>
          {text}
        </AppText>
      </RectButton>
    </Animated.View>
  );
};
