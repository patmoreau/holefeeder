import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppButtonVariant } from '@/shared/presentation/components/AppButtonVariant';
import { AppBottomSheet } from '@/shared/presentation/components/native/AppBottomSheet';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppFieldGroup } from '@/shared/presentation/components/native/AppFieldGroup';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppModifiers } from '@/shared/presentation/components/native/AppModifiers';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppRow } from '@/shared/presentation/components/native/AppRow';
import { AppSpacer } from '@/shared/presentation/components/native/AppSpacer';
import { AppTextInput } from '@/shared/presentation/components/native/AppTextInput';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

type RenameTagModalProps = {
  tag: string;
  isPresented: boolean;
  onCancel: () => void;
  onSave: (newTag: string) => void;
};

export const RenameTagModal = ({ tag, isPresented, onCancel, onSave }: RenameTagModalProps) => {
  const { t } = useTranslation();
  const [name, setName] = useState(tag);

  const canSave = name.trim().length > 0;

  return (
    <AppNative>
      <AppBottomSheet isPresented={isPresented} onDismiss={onCancel} snapPoints={['half']}>
        <AppColumn spacing={16} modifiers={[AppModifiers.fillMaxSize]}>
          <AppFieldGroup>
            <AppFieldSection title={t(tk.manageTags.editTitle)}>
              <AppField icon={AppIconMap.tag} label={t(tk.manageTags.namePlaceholder)} variant="large">
                <AppTextInput placeholder={t(tk.manageTags.namePlaceholder)} value={name} onChangeText={setName} />
              </AppField>
            </AppFieldSection>
          </AppFieldGroup>
          <AppSpacer />
          <AppRow spacing={16} alignment="center" modifiers={[AppModifiers.fillWidth]}>
            <AppSpacer />
            <AppButton variant={AppButtonVariant.secondary} label={t(tk.common.cancel)} onPress={onCancel} />
            <AppButton variant={AppButtonVariant.primary} label={t(tk.common.save)} disabled={!canSave} onPress={() => onSave(name.trim())} />
            <AppSpacer />
          </AppRow>
          <AppSpacer />
        </AppColumn>
      </AppBottomSheet>
    </AppNative>
  );
};
