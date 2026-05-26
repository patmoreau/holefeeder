import { Text } from '@expo/ui/swift-ui';
import { font, foregroundStyle, frame, padding } from '@expo/ui/swift-ui/modifiers';
import { useTranslation } from 'react-i18next';
import { useWindowDimensions } from 'react-native';
import { tk } from '@/i18n/translations';
import { ErrorKey, tkErrorMessages, tkErrorTitles } from '@/shared/core/error-key';
import { AppIcon } from '../../core/app-icons';
import { ExpoBottomSheet } from '../expo/ExpoBottomSheet';
import { ExpoColumn } from '../expo/ExpoColumn';
import { ExpoIcon } from '../expo/ExpoIcon';
import { ExpoRow } from '../expo/ExpoRow';
import { AppButton } from './AppButton';

type AppErrorSheetProps = {
  showError: boolean;
  setShowError: (isOpened: boolean) => void;
  error: ErrorKey;
  onRetry?: () => void;
};
export const AppErrorSheet = ({ showError, setShowError, error, onRetry }: AppErrorSheetProps) => {
  const { t } = useTranslation();
  const { width } = useWindowDimensions();

  return (
    <ExpoBottomSheet isPresented={showError} onDismiss={() => setShowError(false)}>
      <ExpoColumn spacing={32} modifiers={[frame({ maxWidth: width }), padding({ all: 32 })]}>
        <Text modifiers={[font({ size: 24, weight: 'bold' }), foregroundStyle('red')]}>{t(tkErrorTitles[error])}</Text>
        <ExpoRow spacing={16}>
          <ExpoIcon name={AppIcon.warning} />
          <Text modifiers={[font({ size: 17 }), frame({ maxWidth: width, height: 100 })]}>{t(tkErrorMessages[error])}</Text>
        </ExpoRow>
        <ExpoRow spacing={16}>
          {onRetry && <AppButton label={t(tk.errorSheet.retry)} variant="primary" onPress={onRetry} />}
          <AppButton label={t(tk.errorSheet.dismiss)} variant="secondary" onPress={() => setShowError(false)} />
        </ExpoRow>
      </ExpoColumn>
    </ExpoBottomSheet>
  );
};
