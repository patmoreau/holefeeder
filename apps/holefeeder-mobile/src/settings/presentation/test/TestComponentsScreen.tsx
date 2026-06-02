import { Icon, Row, Text } from '@expo/ui';
import { Section, Button } from '@expo/ui/swift-ui';
import { Dispatch, SetStateAction } from 'react';
import { AppBottomSheet } from '@/shared/presentation/components/native/AppBottomSheet';
import { AppFieldGroup } from '@/shared/presentation/components/native/AppFieldGroup';
import { AppList } from '@/shared/presentation/components/native/AppList';
import { AppListItem } from '@/shared/presentation/components/native/AppListItem';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppSwipeActions } from '@/shared/presentation/components/native/AppSwipeActions';
import { TestComponentsButtonSection } from './TestComponentsButtonSection';
import { TestComponentsChipSection } from './TestComponentsChipSection';
import { TestComponentsIconSymbolSection } from './TestComponentsIconSymbolSection';
import { TestComponentsLoadingIndicatorSection } from './TestComponentsLoadingIndicatorSection';
import { TestComponentsPickerSection } from './TestComponentsPickerSection';
import { TestComponentsSwitchSection } from './TestComponentsSwitchSection';
import { TestComponentsTextSection } from './TestComponentsTextSection';

const TestComponentsScreen = ({ show, setShow }: { show: boolean; setShow: Dispatch<SetStateAction<boolean>> }) => {
  return (
    <AppNative matchContents>
      <AppBottomSheet isPresented={show} onDismiss={() => setShow(false)} snapPoints={['half']}>
        <AppFieldGroup>
          <TestComponentsButtonSection />
          <TestComponentsChipSection />
          <TestComponentsPickerSection />
          <TestComponentsSwitchSection />
          <TestComponentsTextSection />
          <TestComponentsIconSymbolSection />
          <TestComponentsLoadingIndicatorSection />
          <AppList>
            <Section>
              <AppListItem key={1} onPress={() => {}}>
                <AppListItem.Leading>
                  <Icon name="star.fill" size={20} color="#FFD60A" />
                </AppListItem.Leading>
                <Row spacing={0}>
                  <Text textStyle={{ color: 'gray' }}>{`#42: `}</Text>
                  <Text>Composite headline</Text>
                </Row>
                <AppListItem.Supporting>Richer slot content</AppListItem.Supporting>
                <AppSwipeActions>
                  <Text>Message from Expo</Text>

                  <AppSwipeActions.Actions edge="leading" allowsFullSwipe={false}>
                    <Button label="Pin" systemImage="pin" onPress={() => {}} />
                  </AppSwipeActions.Actions>

                  <AppSwipeActions.Actions edge="trailing">
                    <Button label="Delete" systemImage="trash" role="destructive" onPress={() => {}} />
                  </AppSwipeActions.Actions>
                </AppSwipeActions>
              </AppListItem>
            </Section>
            <Section>
              <AppListItem key={2} onPress={() => {}}>
                <AppListItem.Leading>
                  <Icon name="star.fill" size={20} color="#FFD60A" />
                </AppListItem.Leading>
                <Row spacing={0}>
                  <Text textStyle={{ color: 'gray' }}>{`#42: `}</Text>
                  <Text>Composite headline</Text>
                </Row>
                <AppListItem.Supporting>Richer slot content</AppListItem.Supporting>
                <AppSwipeActions>
                  <Text>Message from Expo</Text>

                  <AppSwipeActions.Actions edge="leading" allowsFullSwipe={false}>
                    <Button label="Pin" systemImage="pin" onPress={() => {}} />
                  </AppSwipeActions.Actions>

                  <AppSwipeActions.Actions edge="trailing">
                    <Button label="Delete" systemImage="trash" role="destructive" onPress={() => {}} />
                    <Button label="Delete" systemImage="trash" role="destructive" onPress={() => {}} />
                  </AppSwipeActions.Actions>
                </AppSwipeActions>
              </AppListItem>
              <AppListItem key={3} onPress={() => {}}>
                <AppListItem.Leading>
                  <Icon name="star.fill" size={20} color="#FFD60A" />
                </AppListItem.Leading>
                <Row spacing={0}>
                  <Text textStyle={{ color: 'gray' }}>{`#42: `}</Text>
                  <Text>Composite headline</Text>
                </Row>
                <AppListItem.Supporting>Richer slot content</AppListItem.Supporting>
                <AppSwipeActions>
                  <Text>Message from Expo</Text>

                  <AppSwipeActions.Actions edge="leading" allowsFullSwipe={false}>
                    <Button label="Pin" systemImage="pin" onPress={() => {}} />
                  </AppSwipeActions.Actions>

                  <AppSwipeActions.Actions edge="trailing">
                    <Button label="Delete" systemImage="trash" role="destructive" onPress={() => {}} />
                    <Button label="Delete" systemImage="trash" role="destructive" onPress={() => {}} />
                  </AppSwipeActions.Actions>
                </AppSwipeActions>
              </AppListItem>
            </Section>
          </AppList>
        </AppFieldGroup>
      </AppBottomSheet>
    </AppNative>
  );
};

export default TestComponentsScreen;
