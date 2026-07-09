import React, { ReactNode, useRef } from 'react';
import { View } from 'react-native';
import Swipeable, { SwipeableMethods, SwipeDirection } from 'react-native-gesture-handler/ReanimatedSwipeable';
import { SharedValue } from 'react-native-reanimated';
import { useStyles } from '@/shared/theme/core/use-styles';

type AppSwipeableRowProps = {
  renderLeftActions?: (progress: SharedValue<number>, swipeableRef: React.RefObject<SwipeableMethods | null>) => ReactNode;
  renderRightActions?: (progress: SharedValue<number>, swipeableRef: React.RefObject<SwipeableMethods | null>) => ReactNode;
  onSwipeableLeftOpen?: () => void;
  children?: ReactNode;
};

const createStyles = () => ({
  rightActionsView: {
    width: 192,
    flexDirection: 'row' as const,
  },
});

export const AppSwipeableRow = ({ renderLeftActions, renderRightActions, onSwipeableLeftOpen, children }: AppSwipeableRowProps) => {
  const styles = useStyles(createStyles);
  const swipeableRow = useRef<SwipeableMethods>(null);

  const rightActions = (renderRightActions: ReactNode) => <View style={styles.rightActionsView}>{renderRightActions}</View>;

  return (
    <Swipeable
      ref={swipeableRow}
      friction={2}
      enableTrackpadTwoFingerGesture
      leftThreshold={30}
      rightThreshold={40}
      renderLeftActions={renderLeftActions ? (_, progress) => renderLeftActions(progress, swipeableRow) : undefined}
      renderRightActions={renderRightActions ? (_, progress) => rightActions(renderRightActions(progress, swipeableRow)) : undefined}
      onSwipeableOpen={(direction) => {
        if (direction === SwipeDirection.RIGHT) {
          onSwipeableLeftOpen?.();
          swipeableRow.current?.close();
        }
      }}
    >
      {children}
    </Swipeable>
  );
};
