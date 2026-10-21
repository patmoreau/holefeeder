import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppRow } from '@/shared/presentation/components/native/AppRow';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { UniversalIcon } from '@/shared/presentation/core/app-icon-map';
import { useTheme } from '@/shared/theme/core/use-theme';
import { borderRadius } from '@/types/theme/design-tokens';

export type TrendBadgeTone = 'positive' | 'negative';

export const TrendBadge = ({ text, tone, icon }: { text: string; tone: TrendBadgeTone; icon: UniversalIcon }) => {
  const { theme } = useTheme();

  const color = tone === 'negative' ? theme.colors.negative : theme.colors.positive;
  const backgroundColor = tone === 'negative' ? theme.colors.negativeBackground : theme.colors.positiveBackground;

  return (
    <AppColumn
      style={{
        borderRadius: borderRadius.xl,
        backgroundColor: backgroundColor + '80',
        borderColor: backgroundColor + '90',
      }}
    >
      <AppRow spacing={8} style={{ padding: 8 }} alignment={'center'}>
        <AppIcon name={icon} color={color} size={14} />
        <AppText textStyle={{ color }}>{text}</AppText>
      </AppRow>
    </AppColumn>
  );
};
