import { Dispatch, SetStateAction } from 'react';
import { AppBottomSheet } from '@/shared/presentation/components/native/AppBottomSheet';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppFieldGroup } from '@/shared/presentation/components/native/AppFieldGroup';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppList } from '@/shared/presentation/components/native/AppList';
import { AppListItem } from '@/shared/presentation/components/native/AppListItem';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppRow } from '@/shared/presentation/components/native/AppRow';
import { AppSection } from '@/shared/presentation/components/native/AppSection';
import { AppSwipeActions } from '@/shared/presentation/components/native/AppSwipeActions';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
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
            <AppSection>
              <AppListItem key={1} onPress={() => {}}>
                <AppListItem.Leading>
                  <AppIcon name="star.fill" size={20} color="#FFD60A" />
                </AppListItem.Leading>
                <AppRow spacing={0}>
                  <AppText textStyle={{ color: 'gray' }}>{`#42: `}</AppText>
                  <AppText>Composite headline</AppText>
                </AppRow>
                <AppListItem.Supporting>Richer slot content</AppListItem.Supporting>
                <AppSwipeActions>
                  <AppText>Message from Expo</AppText>

                  <AppSwipeActions.Actions edge="leading" allowsFullSwipe={false}>
                    <AppButton variant="secondary" label="Pin" onPress={() => {}} />
                  </AppSwipeActions.Actions>

                  <AppSwipeActions.Actions edge="trailing">
                    <AppButton variant="destructive" label="Delete" icon={AppIconMap.delete} onPress={() => {}} />
                  </AppSwipeActions.Actions>
                </AppSwipeActions>
              </AppListItem>
            </AppSection>
            <AppSection>
              <AppListItem key={2} onPress={() => {}}>
                <AppListItem.Leading>
                  <AppIcon name="star.fill" size={20} color="#FFD60A" />
                </AppListItem.Leading>
                <AppRow spacing={0}>
                  <AppText textStyle={{ color: 'gray' }}>{`#42: `}</AppText>
                  <AppText>Composite headline</AppText>
                </AppRow>
                <AppListItem.Supporting>Richer slot content</AppListItem.Supporting>
                <AppSwipeActions>
                  <AppText>Message from Expo</AppText>

                  <AppSwipeActions.Actions edge="leading" allowsFullSwipe={false}>
                    <AppButton variant="secondary" label="Pin" onPress={() => {}} />
                  </AppSwipeActions.Actions>

                  <AppSwipeActions.Actions edge="trailing">
                    <AppButton variant="destructive" label="Delete" icon={AppIconMap.delete} onPress={() => {}} />
                    <AppButton variant="destructive" label="Delete" icon={AppIconMap.delete} onPress={() => {}} />
                  </AppSwipeActions.Actions>
                </AppSwipeActions>
              </AppListItem>
              <AppListItem key={3} onPress={() => {}}>
                <AppListItem.Leading>
                  <AppIcon name="star.fill" size={20} color="#FFD60A" />
                </AppListItem.Leading>
                <AppRow spacing={0}>
                  <AppText textStyle={{ color: 'gray' }}>{`#42: `}</AppText>
                  <AppText>Composite headline</AppText>
                </AppRow>
                <AppListItem.Supporting>Richer slot content</AppListItem.Supporting>
                <AppSwipeActions>
                  <AppText>Message from Expo</AppText>

                  <AppSwipeActions.Actions edge="leading" allowsFullSwipe={false}>
                    <AppButton variant="secondary" label="Pin" onPress={() => {}} />
                  </AppSwipeActions.Actions>

                  <AppSwipeActions.Actions edge="trailing">
                    <AppButton variant="destructive" label="Delete" icon={AppIconMap.delete} onPress={() => {}} />
                    <AppButton variant="destructive" label="Delete" icon={AppIconMap.delete} onPress={() => {}} />
                  </AppSwipeActions.Actions>
                </AppSwipeActions>
              </AppListItem>
            </AppSection>
          </AppList>
        </AppFieldGroup>
      </AppBottomSheet>
    </AppNative>
  );
};

export default TestComponentsScreen;
