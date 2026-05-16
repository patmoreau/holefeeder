import { Stack, useNavigation } from 'expo-router';
import React, { useState } from 'react';
import { View } from 'react-native';
import { today } from '@/shared/core/with-date';
import { AppField } from '@/shared/presentation/AppField';
import { AppForm } from '@/shared/presentation/AppForm';
import { AppScreen } from '@/shared/presentation/AppScreen';
import { AppSection } from '@/shared/presentation/AppSection';
import { AppButton } from '@/shared/presentation/components/AppButton';
import { AppChip } from '@/shared/presentation/components/AppChip';
import { AppDatePicker } from '@/shared/presentation/components/AppDatePicker';
import { AppPicker, PickerOption } from '@/shared/presentation/components/AppPicker';
import { AppSwitch } from '@/shared/presentation/components/AppSwitch';
import { AppText } from '@/shared/presentation/components/AppText';
import { AppTextInput } from '@/shared/presentation/components/AppTextInput';
import { ErrorSheet } from '@/shared/presentation/components/ErrorSheet';
import { LoadingIndicator } from '@/shared/presentation/components/LoadingIndicator';
import { AppIcons } from '@/shared/presentation/icons';

type PickerType<T> = PickerOption & { value: T };
const pickerOptions: PickerType<string>[] = [
  { id: 'menu1', value: 'menu' },
  { id: 'menu2', value: 'medium menu' },
  { id: 'menu3', value: 'a very incredibly super super very long menu' },
];
const iconOptions: PickerType<AppIcons>[] = Object.entries(AppIcons).map((icon) => ({ id: icon[0], value: icon[1] }));

const TestComponentsScreen = () => {
  const [chipSelection, setChipSelection] = useState(false);
  const [date, setDate] = useState(today());
  const [switchValue, setSwitchValue] = useState(false);
  const [pickerValue, setPickerValue] = useState<PickerType<string>>(pickerOptions[0]);
  const [textValue, setTextValue] = useState('');
  const [showError, setShowError] = useState(false);
  const [iconValue, setIconValue] = useState<PickerType<AppIcons>>(iconOptions[0]);
  const navigation = useNavigation();

  return (
    <>
      <Stack.Toolbar placement="left">
        <Stack.Toolbar.Button icon={AppIcons.back} onPress={() => navigation.goBack()} />
      </Stack.Toolbar>

      <AppScreen style={{ paddingTop: 80, paddingBottom: 32 }}>
        <AppForm>
          <AppSection title={'AppButton'}>
            <AppField icon={AppIcons.add} label="primary">
              <View style={{ flex: 1, flexDirection: 'column', alignItems: 'center', alignContent: 'flex-end' }}>
                <AppButton icon={AppIcons.add} variant="primary" onPress={() => alert('Test Button Clicked')} />
                <AppButton icon={AppIcons.add} label="Click me!" variant="primary" onPress={() => alert('Test Button Clicked')} />
                <AppButton label="Click me!" variant="primary" onPress={() => alert('Test Button Clicked')} />
              </View>
            </AppField>
            <AppField icon={AppIcons.add} label="secondary">
              <AppButton label="Click me!" variant="secondary" onPress={() => alert('Test Button Clicked')} />
            </AppField>
            <AppField icon={AppIcons.add} label="destructive">
              <AppButton label="Click me!" variant="destructive" onPress={() => alert('Test Button Clicked')} />
            </AppField>
            <AppField icon={AppIcons.add} label="link">
              <AppButton label="Click me!" variant="link" onPress={() => alert('Test Button Clicked')} />
            </AppField>
          </AppSection>
          <AppSection title={'AppChip'}>
            <AppField icon={AppIcons.back} label="AppChip">
              <AppChip label="Test Button" selected={chipSelection} onPress={() => setChipSelection(!chipSelection)} />
            </AppField>
          </AppSection>
          <AppSection title={'AppDatePicker'}>
            <AppField icon={AppIcons.account} label="AppDatePicker">
              <AppDatePicker selectedDate={date} onDateSelected={(d) => setDate(d)} />
            </AppField>
          </AppSection>
          <AppSection title={'AppPicker'}>
            <AppField icon={AppIcons.category} label="Menu">
              <AppPicker
                variant={'menu'}
                options={pickerOptions}
                onOptionLabel={(option) => option.value}
                selectedOption={pickerValue}
                onSelectOption={setPickerValue}
              />
            </AppField>
            <AppField icon={AppIcons.category} label="Segmented" variant={'large'}>
              <AppPicker
                variant={'segmented'}
                options={pickerOptions}
                onOptionLabel={(option) => option.value}
                selectedOption={pickerValue}
                onSelectOption={setPickerValue}
              />
            </AppField>
          </AppSection>
          <AppSection title={'AppSwitch'}>
            <AppField icon={AppIcons.expand} label="Switch">
              <AppSwitch value={switchValue} onChange={setSwitchValue} />
            </AppField>
          </AppSection>
          <AppSection title={'AppText'}>
            <AppField icon={AppIcons.description} label={'Default'}>
              <AppText variant={'default'}>Default</AppText>
            </AppField>
            <AppField icon={AppIcons.description} label={'Large Title'}>
              <AppText variant={'largeTitle'}>Large Title</AppText>
            </AppField>
            <AppField icon={AppIcons.description} label={'Title'}>
              <AppText variant={'title'}>Title</AppText>
            </AppField>
            <AppField icon={AppIcons.description} label={'Default Semi Bold'}>
              <AppText variant={'defaultSemiBold'}>Default Semi Bold</AppText>
            </AppField>
            <AppField icon={AppIcons.description} label={'Subtitle'}>
              <AppText variant={'subtitle'}>Subtitle</AppText>
            </AppField>
            <AppField icon={AppIcons.description} label={'Link'}>
              <AppText variant={'link'}>Link</AppText>
            </AppField>
            <AppField icon={AppIcons.description} label={'Footnote'}>
              <AppText variant={'footnote'}>Footnote</AppText>
            </AppField>
          </AppSection>
          <AppSection title={'AppTextInput'}>
            <AppField icon={AppIcons.description}>
              <AppTextInput
                placeholder={'write somethings here...'}
                value={textValue}
                onChangeText={setTextValue}
                onSubmitEditing={() => alert(`Text input submitted: ${textValue}`)}
              />
            </AppField>
          </AppSection>
          <AppSection title={'ErrorSheet'}>
            <AppField icon={AppIcons.warning}>
              <AppSwitch value={showError} onChange={setShowError} />
            </AppField>
          </AppSection>
          <AppSection title={'IconSymbol'}>
            <AppField icon={iconValue.value}>
              <AppPicker
                variant={'menu'}
                options={iconOptions}
                onOptionLabel={(option) => option.id as string}
                selectedOption={iconValue}
                onSelectOption={setIconValue}
              />
            </AppField>
          </AppSection>
          <AppSection title={'LoadingIndicator'}>
            <AppField icon={AppIcons.tag} label={'Large'}>
              <LoadingIndicator variant={'primary'} size={'large'} />
            </AppField>
            <AppField icon={AppIcons.tag} label={'Small'}>
              <LoadingIndicator variant={'secondary'} size={'small'} />
            </AppField>
          </AppSection>
        </AppForm>
        <ErrorSheet showError={showError} setShowError={setShowError} error={'noInternetConnection'} />
      </AppScreen>
    </>
  );
};

export default TestComponentsScreen;
