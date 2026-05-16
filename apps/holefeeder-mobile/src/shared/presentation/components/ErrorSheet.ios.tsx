import { BottomSheet, Button, HStack, Image, Text, VStack } from '@expo/ui/swift-ui';
import { buttonStyle, font, foregroundStyle, frame, padding } from '@expo/ui/swift-ui/modifiers';
import * as React from 'react';
import { useTranslation } from 'react-i18next';
import { useWindowDimensions } from 'react-native';
import { tk } from '@/i18n/translations';
import { ErrorKey, tkErrorMessages, tkErrorTitles } from '@/shared/core/error-key';
import { AppHost } from '@/shared/presentation/components/AppHost.ios';

type Props = {
  showError: boolean;
  setShowError: (isOpened: boolean) => void;
  error: ErrorKey;
  onRetry?: () => void;
};
export const ErrorSheet = ({ showError, setShowError, error, onRetry }: Props) => {
  const { t } = useTranslation();
  const { width } = useWindowDimensions();

  return (
    <AppHost style={{ position: 'absolute', width }}>
      <BottomSheet isPresented={showError} onIsPresentedChange={(e) => setShowError(e)}>
        <VStack spacing={32} modifiers={[frame({ maxWidth: width }), padding({ all: 32 })]}>
          <Text modifiers={[font({ size: 24, weight: 'bold' }), foregroundStyle('red')]}>{t(tkErrorTitles[error])}</Text>
          <HStack spacing={16}>
            <Image systemName="exclamationmark.triangle" />
            <Text modifiers={[font({ size: 17 }), frame({ maxWidth: width, height: 100 })]}>{t(tkErrorMessages[error])}</Text>
          </HStack>
          <HStack spacing={16}>
            {onRetry && <Button label={t(tk.errorSheet.retry)} modifiers={[buttonStyle('borderedProminent')]} onPress={onRetry} />}
            <Button label={t(tk.errorSheet.dismiss)} modifiers={[buttonStyle('bordered')]} onPress={() => setShowError(false)} />
          </HStack>
        </VStack>
      </BottomSheet>
    </AppHost>
  );
};
