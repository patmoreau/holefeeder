import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { ParallaxScrollView } from '@/shared/presentation/ParallaxScrollView';
import { ScreenTitle } from '@/shared/presentation/ScreenTitle';
import { useTheme } from '@/shared/theme/core/use-theme';
import { CategorySpendingList } from './CategorySpendingList';

export default function InsightsScreen() {
  const { theme } = useTheme();
  const { t } = useTranslation();

  return (
    <ParallaxScrollView
      headerBackgroundColor={theme.colors.primary}
      headerImage={
        <AppNative>
          <AppIcon size={310} color="#808080" name={AppIconMap.insights} />
        </AppNative>
      }
    >
      <ScreenTitle title={t(tk.insights.title)} />
      <CategorySpendingList />
    </ParallaxScrollView>
  );
}
