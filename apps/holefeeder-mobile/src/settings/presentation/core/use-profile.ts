import { useEffect, useState } from 'react';
import { UserProfile } from '@/settings/presentation/core/user-profile';
import { useAuth } from '@/shared/auth/core/use-auth';

export const initialProfile: UserProfile = {
  name: '',
  username: '',
  email: '',
  avatar: 'person.fill',
};

export const useProfile = (): UserProfile => {
  const { user } = useAuth();
  const [profile, setProfile] = useState<UserProfile>(initialProfile);

  useEffect(() => {
    if (user) {
      setProfile({
        name: user.name || `${user.givenName} ${user.familyName}` || '',
        username: user.sub || '',
        email: user.email || '',
        avatar: user.picture || 'person.fill',
      });
    } else {
      setProfile(initialProfile);
    }
  }, [user]);

  return profile;
};
