import { Image as ExpoImage } from 'expo-image';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AVATAR_SIZE, useProfile } from '@/settings/presentation/core/use-profile';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppReact } from '@/shared/presentation/components/native/AppReact';
import { AppRow } from '@/shared/presentation/components/native/AppRow';
import { AppSpacer } from '@/shared/presentation/components/native/AppSpacer';
import { AppText } from '@/shared/presentation/components/native/AppText';

const styles = {
  avatar: {
    width: AVATAR_SIZE,
    height: AVATAR_SIZE,
    borderRadius: AVATAR_SIZE / 2,
  },
};

export const ProfileSection = () => {
  const profile = useProfile();
  const { t } = useTranslation();

  return (
    <AppFieldSection title={t(tk.profileSection.title)}>
      <AppRow spacing={14} alignment={'center'}>
        <AppReact matchContents>
          <ExpoImage source={{ uri: profile.avatar }} style={styles.avatar} contentFit="cover" />
        </AppReact>
        <AppColumn spacing={2} alignment={'start'}>
          <AppText variant={'defaultSemiBold'} numberOfLines={1}>
            {profile.name}
          </AppText>
          <AppText variant={'footnote'} numberOfLines={1}>
            {profile.email}
          </AppText>
        </AppColumn>
        <AppSpacer />
      </AppRow>
    </AppFieldSection>
  );
};
