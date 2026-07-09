import { useContext } from 'react';
import { LanguageContext } from '@/shared/language/presentation/LanguageProvider';

export const useLanguage = () => {
  const state = useContext(LanguageContext);
  if (!state) throw new Error('useLanguage must be used within a LanguageProvider');
  return state;
};
