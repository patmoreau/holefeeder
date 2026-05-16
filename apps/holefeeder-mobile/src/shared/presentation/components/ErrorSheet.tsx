import React from 'react';
import { useTranslation } from 'react-i18next';
import { Text, View } from 'react-native';
import { tk } from '@/i18n/translations';
import { ErrorKey, tkErrorMessages, tkErrorTitles } from '@/shared/core/error-key';
import { AppButton } from '@/shared/presentation/components/AppButton';
import { fontSize, fontWeight, spacing } from '@/types/theme/design-tokens';

type Props = {
  showError: boolean;
  setShowError: (isOpened: boolean) => void;
  error: ErrorKey;
  onRetry?: () => void;
};

export const ErrorSheet = ({ showError, setShowError, error, onRetry }: Props) => {
  const { t } = useTranslation();

  if (!showError) return null;

  return (
    <View
      style={{
        position: 'absolute',
        bottom: 0,
        left: 0,
        right: 0,
        backgroundColor: 'white',
        padding: spacing.lg,
        borderTopLeftRadius: 12,
        borderTopRightRadius: 12,
        shadowColor: '#000',
        shadowOpacity: 0.2,
        shadowRadius: 8,
        elevation: 6,
      }}
    >
      <Text style={{ fontSize: fontSize!.xl, fontWeight: fontWeight.semiBold, marginBottom: spacing.sm }}>{t(tkErrorTitles[error])}</Text>
      <Text style={{ fontSize: fontSize!.md, marginBottom: spacing.lg }}>{t(tkErrorMessages[error])}</Text>
      <View style={{ flexDirection: 'row', justifyContent: 'flex-end', gap: spacing.md }}>
        {onRetry && <AppButton label={t(tk.errorSheet.retry)} variant={'primary'} onPress={onRetry} />}
        <AppButton label={t(tk.errorSheet.dismiss)} variant={'secondary'} onPress={() => setShowError(false)} />
      </View>
    </View>
  );
};
