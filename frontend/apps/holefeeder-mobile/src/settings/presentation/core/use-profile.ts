import { UserProfile } from '@/settings/presentation/core/user-profile';
import { useAuth } from '@/shared/auth/core/use-auth';

export const initialProfile: UserProfile = {
  name: '',
  username: '',
  email: '',
  avatar: '',
};

export const AVATAR_SIZE = 52;

const initialsAvatar = (label: string): string =>
  `https://ui-avatars.com/api/?name=${encodeURIComponent(label)}&size=${AVATAR_SIZE * 3}&background=007AFF&color=fff`;

export const useProfile = (): UserProfile => {
  const { user } = useAuth();

  if (!user) {
    return initialProfile;
  }

  const name = user.name || [user.givenName, user.familyName].filter(Boolean).join(' ');
  const email = user.email || '';
  return {
    name: name,
    username: user.sub || '',
    email: email,
    avatar: user.picture || initialsAvatar(name || email || 'User'),
  };
};
