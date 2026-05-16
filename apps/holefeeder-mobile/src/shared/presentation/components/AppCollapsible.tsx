import { PropsWithChildren, useState } from 'react';
import { StyleProp, TouchableOpacity, ViewStyle } from 'react-native';
import { AppView } from '@/shared/presentation/AppView';
import { AppText } from '@/shared/presentation/components/AppText';
import { IconSymbol } from '@/shared/presentation/components/IconSymbol';
import { AppIcons } from '@/shared/presentation/icons';
import { useStyles } from '@/shared/theme/core/use-styles';
import { useTheme } from '@/shared/theme/core/use-theme';
import { spacing, Theme } from '@/types/theme';

const createStyles = (theme: Theme) => ({
  heading: {
    flexDirection: 'row' as const,
    alignItems: 'center' as const,
    justifyContent: 'space-between' as const,
    gap: 6,
    marginHorizontal: spacing.lg,
  },
  content: {
    marginTop: 6,
  },
});

export function AppCollapsible({ children, title, style }: PropsWithChildren & { title: string; style?: StyleProp<ViewStyle> }) {
  const styles = useStyles(createStyles);
  const [isOpen, setIsOpen] = useState(false);
  const { theme } = useTheme();

  return (
    <AppView style={style}>
      <TouchableOpacity style={styles.heading} onPress={() => setIsOpen((value) => !value)} activeOpacity={0.8}>
        <AppText variant="defaultSemiBold">{title}</AppText>
        <IconSymbol
          name={AppIcons.expand}
          size={18}
          weight="medium"
          color={theme.colors.icon}
          style={{ transform: [{ rotate: isOpen ? '90deg' : '0deg' }] }}
        />
      </TouchableOpacity>
      {isOpen && <AppView style={styles.content}>{children}</AppView>}
    </AppView>
  );
}
