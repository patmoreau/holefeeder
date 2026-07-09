import { ZStack, List, Section } from '@expo/ui/swift-ui';
import { frame, opacity, padding, background, listSectionMargins, onGeometryChange } from '@expo/ui/swift-ui/modifiers';
import { Logger } from '@holefeeder/shared/core';
import { createContext, Fragment, ReactNode, useContext, useState } from 'react';
import { onScrollOffsetChange } from '@/modules/app-modifiers';
import { AppNative } from '@/shared/presentation/components/native/AppNative';

const logger = Logger.create('AppAnimatedScrollView');

const TRANSITION_THRESHOLD = 120;
const BIG_HEIGHT = 200;
const SMALL_HEIGHT = 80;

type AppAnimatedScrollContextValue = {
  wrapItem: (child: React.ReactNode, key: string | number) => React.ReactNode;
};

export const AppAnimatedScrollContext = createContext<AppAnimatedScrollContextValue>({
  wrapItem: (child) => child,
});

export const useAppAnimatedScrollItem = () => useContext(AppAnimatedScrollContext);

type Props = {
  largeCard: ReactNode;
  smallCard: ReactNode;
  children: ReactNode;
};

export function AppAnimatedScrollView({ largeCard, smallCard, children }: Props) {
  const [scrollY, setScrollY] = useState(0);

  // 0 = top (big visible) → 1 = scrolled (small visible)
  const progress = Math.min(1, Math.max(0, scrollY / TRANSITION_THRESHOLD));
  const bigOpacity = 1 - progress;
  const smallOpacity = progress;
  const headerHeight = BIG_HEIGHT - progress * (BIG_HEIGHT - SMALL_HEIGHT);

  return (
    <AppAnimatedScrollContext.Provider
      value={{
        wrapItem: (child, key) => <Fragment key={key}>{child}</Fragment>,
      }}
    >
      <AppNative style={{ flex: 1 }}>
        <List
          modifiers={[
            onGeometryChange((g) => {
              logger.debug('onGeometryChange', { g });
            }),
            frame({
              maxWidth: Infinity,
              maxHeight: Infinity,
            }),
            background('transparent'),
            onScrollOffsetChange((offsetY) => {
              logger.debug('onScrollOffsetChange', { offsetY });
              setScrollY(offsetY);
            }),
          ]}
        >
          <Section
            modifiers={[padding({ horizontal: 0 }), listSectionMargins({ length: 0, edges: 'all' })]}
            header={
              <ZStack modifiers={[frame({ height: headerHeight, maxWidth: Infinity }), background('yellow')]}>
                <ZStack modifiers={[frame({ maxWidth: Infinity, maxHeight: Infinity }), opacity(bigOpacity), background('transparent')]}>
                  {largeCard}
                </ZStack>
                <ZStack modifiers={[frame({ maxWidth: Infinity, maxHeight: Infinity }), opacity(smallOpacity), background('transparent')]}>
                  {smallCard}
                </ZStack>
              </ZStack>
            }
          >
            {children}
          </Section>
        </List>
      </AppNative>
    </AppAnimatedScrollContext.Provider>
  );
}
