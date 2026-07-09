import { Stack } from 'expo-router';
import type { ReactElement } from 'react';
import { Text, View, type ViewProps } from 'react-native';
import Animated, { interpolate, useAnimatedRef, useAnimatedStyle, useScrollOffset } from 'react-native-reanimated';
import { AppView } from '@/shared/presentation/AppView';
import { useStyles } from '@/shared/theme/core/use-styles';
import { Theme } from '@/types/theme/theme';

const HEADER_HEIGHT = 250;

type Props = ViewProps & {
  headerImage: ReactElement;
  headerBackgroundColor: string;
};

const createStyles = (theme: Theme) => ({
  container: {
    flex: 1,
  },
  header: {
    height: HEADER_HEIGHT,
    overflow: 'hidden' as const,
  },
  content: {
    minHeight: '100%' as const,
  },
  scrollArea: {
    backgroundColor: theme.colors.background,
    flex: 1,
  },
});

export const ParallaxScrollView = ({ headerImage, headerBackgroundColor, ...otherProps }: Props) => {
  const styles = useStyles(createStyles);

  const scrollRef = useAnimatedRef<Animated.ScrollView>();
  const scrollOffset = useScrollOffset(scrollRef);

  const headerAnimatedStyle = useAnimatedStyle(() => {
    return {
      transform: [
        {
          translateY: interpolate(scrollOffset.value, [-HEADER_HEIGHT, 0, HEADER_HEIGHT], [-HEADER_HEIGHT / 2, 0, HEADER_HEIGHT * 0.75]),
        },
        {
          scale: interpolate(scrollOffset.value, [-HEADER_HEIGHT, 0, HEADER_HEIGHT], [2, 1, 1]),
        },
      ],
    };
  });

  return (
    <View style={styles.container}>
      <Stack.Screen
        options={{
          headerTransparent: true,
          headerLeft: () => <Text>Back</Text>,
          headerBackground: () => <Animated.View style={[styles.header, headerAnimatedStyle]} />,
        }}
      />

      <Animated.ScrollView ref={scrollRef} style={styles.scrollArea} scrollEventThrottle={16} removeClippedSubviews={false}>
        <Animated.View style={[styles.header, { backgroundColor: headerBackgroundColor }, headerAnimatedStyle]}>{headerImage}</Animated.View>
        <AppView style={styles.content} {...otherProps}></AppView>
      </Animated.ScrollView>
    </View>
  );
};
