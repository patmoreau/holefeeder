import { Icon, List, ListItem, Row, Text } from '@expo/ui';
import { Button, Section, SwipeActions } from '@expo/ui/swift-ui';
import { Dispatch, SetStateAction } from 'react';
import { AppBottomSheet } from '@/shared/presentation/components/app/AppBottomSheet';
import { AppFieldGroup } from '@/shared/presentation/components/app/AppFieldGroup';
import { AppHost } from '@/shared/presentation/components/app/AppHost';
import { TestComponentsButtonSection } from './TestComponentsButtonSection';
import { TestComponentsChipSection } from './TestComponentsChipSection';
import { TestComponentsIconSymbolSection } from './TestComponentsIconSymbolSection';
import { TestComponentsLoadingIndicatorSection } from './TestComponentsLoadingIndicatorSection';
import { TestComponentsPickerSection } from './TestComponentsPickerSection';
import { TestComponentsSwitchSection } from './TestComponentsSwitchSection';
import { TestComponentsTextSection } from './TestComponentsTextSection';

const TestComponentsScreen = ({ show, setShow }: { show: boolean; setShow: Dispatch<SetStateAction<boolean>> }) => {
  return (
    <AppHost matchContents>
      <AppBottomSheet isPresented={show} onDismiss={() => setShow(false)} snapPoints={['half']}>
        <AppFieldGroup>
          <TestComponentsButtonSection />
          <TestComponentsChipSection />
          <TestComponentsPickerSection />
          <TestComponentsSwitchSection />
          <TestComponentsTextSection />
          <TestComponentsIconSymbolSection />
          <TestComponentsLoadingIndicatorSection />
          <List>
            <ListItem onPress={() => {}}>
              <ListItem.Leading>
                <Icon name="star.fill" size={20} color="#FFD60A" />
              </ListItem.Leading>
              <Row spacing={0}>
                <Text textStyle={{ color: 'gray' }}>{`#42: `}</Text>
                <Text>Composite headline</Text>
              </Row>
              <ListItem.Supporting>Richer slot content</ListItem.Supporting>
            </ListItem>
            <Section>
              <SwipeActions>
                <Text>Message from Expo</Text>

                <SwipeActions.Actions edge="leading" allowsFullSwipe={false}>
                  <Button label="Pin" systemImage="pin" onPress={() => {}} />
                </SwipeActions.Actions>

                <SwipeActions.Actions edge="trailing">
                  <Button label="Delete" systemImage="trash" role="destructive" onPress={() => {}} />
                </SwipeActions.Actions>
              </SwipeActions>
            </Section>
          </List>
        </AppFieldGroup>
      </AppBottomSheet>
    </AppHost>
  );
};

export default TestComponentsScreen;
