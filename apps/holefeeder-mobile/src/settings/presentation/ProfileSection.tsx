import { Image as ExpoImage } from 'expo-image';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { tk } from '@/i18n/translations';
import { useProfile } from '@/settings/presentation/core/use-profile';
import { AppSection } from '@/shared/presentation/AppSection';
import { AuthButton } from '@/shared/presentation/AuthButton';
import { AppText } from '@/shared/presentation/components/AppText';
import { useStyles } from '@/shared/theme/core/use-styles';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  profile: {
    flexDirection: 'row' as const,
    paddingVertical: 8,
  },
  avatarContainer: {
    width: 120,
    height: 120,
    borderRadius: 100,
    margin: 8,
  },
  avatar: {
    width: 120,
    height: 120,
    borderRadius: 60,
    borderWidth: 3,
    borderColor: theme.colors.primary,
  },
  profileInfo: {
    flexDirection: 'column' as const,
    flex: 1,
    gap: 4,
    alignItems: 'center' as const,
    paddingVertical: 8,
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
    <AppSection title={t(tk.profileSection.title)}>
      <View style={styles.profile}>
        <View style={styles.avatarContainer}>
          <ExpoImage source={{ uri: avatarUri }} style={styles.avatar} contentFit="cover" />
        </View>

        <View style={styles.profileInfo}>
          <AppText variant={'title'}>{profile.name}</AppText>
          <AppText>{profile.email}</AppText>
          <AuthButton />
        </View>
      </View>
    </AppSection>
  );
};
