import { useNativeState } from '@expo/ui';
import { useEffectEvent } from 'react';
import { AppIconMap } from '@/shared/presentation/components/app/app-icon-map';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppFieldSection } from '@/shared/presentation/components/app/AppFieldSection';
import { AppText } from '@/shared/presentation/components/app/AppText';
import { AppTextInput } from '@/shared/presentation/components/app/AppTextInput';

export const TestComponentsTextSection = () => {
  const text = useNativeState('');

  const handleChangeText = useEffectEvent((value: string) => {
    'worklet';
    text.value = value;
  });

  return (
    <AppFieldSection title={'AppChip'}>
      <AppField icon={AppIconMap.description} label={'Default'}>
        <AppText variant={'default'}>Default</AppText>
      </AppField>
      <AppField icon={AppIconMap.description} label={'Large Title'}>
        <AppText variant={'largeTitle'}>Large Title</AppText>
      </AppField>
      <AppField icon={AppIconMap.description} label={'Title'}>
        <AppText variant={'title'}>Title</AppText>
      </AppField>
      <AppField icon={AppIconMap.description} label={'Default Semi Bold'}>
        <AppText variant={'defaultSemiBold'}>Default Semi Bold</AppText>
      </AppField>
      <AppField icon={AppIconMap.description} label={'Subtitle'}>
        <AppText variant={'subtitle'}>Subtitle</AppText>
      </AppField>
      <AppField icon={AppIconMap.description} label={'Link'}>
        <AppText variant={'link'}>Link</AppText>
      </AppField>
      <AppField icon={AppIconMap.description} label={'Footnote'}>
        <AppText variant={'footnote'}>Footnote</AppText>
      </AppField>
      <AppField icon={AppIconMap.description} label={'TextInput'} variant="large">
        <AppTextInput
          placeholder={'write somethings here...'}
          value={text.value}
          onChangeText={handleChangeText}
          onSubmitEditing={() => alert(`Text input submitted: ${text.value}`)}
        />
      </AppField>
    </AppFieldSection>
  );
};
