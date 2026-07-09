import { Image as ExpoImage } from 'expo-image';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { useProfile } from '@/settings/presentation/core/use-profile';
import { AuthButton } from '@/shared/presentation/AuthButton';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppReact } from '@/shared/presentation/components/native/AppReact';
import { AppRow } from '@/shared/presentation/components/native/AppRow';
import { AppSpacer } from '@/shared/presentation/components/native/AppSpacer';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { useStyles } from '@/shared/theme/core/use-styles';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  avatar: {
    width: 120,
    height: 120,
    borderRadius: 60,
    borderWidth: 3,
    borderColor: theme.colors.primary,
  },
});

export const ProfileSection = () => {
  const profile = useProfile();
  const { t } = useTranslation();
  const styles = useStyles(createStyles);

  const avatarUri =
    profile.avatar ||
    `https://ui-avatars.com/api/?name=${encodeURIComponent(profile.name || profile.email || 'User')}&size=120&background=007AFF&color=fff`;

  return (
    <AppFieldSection title={t(tk.profileSection.title)}>
      <AppRow>
        <AppReact matchContents>
          <ExpoImage source={{ uri: avatarUri }} style={styles.avatar} contentFit="cover" />
        </AppReact>
        <AppSpacer />
        <AppColumn>
          <AppText variant={'title'}>{profile.name}</AppText>
          <AppText>{profile.email}</AppText>
          <AuthButton />
        </AppColumn>
      </AppRow>
    </AppFieldSection>
  );
};
