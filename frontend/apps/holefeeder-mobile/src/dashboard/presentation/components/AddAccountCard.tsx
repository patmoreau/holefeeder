import { Image } from 'expo-image';
import { SymbolView } from 'expo-symbols';
import { useTranslation } from 'react-i18next';
import { Pressable, StyleSheet, View } from 'react-native';
import { tk } from '@/i18n/translations';
import { AppCard } from '@/shared/presentation/components/AppCard';
import { AppText } from '@/shared/presentation/components/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useStyles } from '@/shared/theme/core/use-styles';
import { borderRadius, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

export type AddAccountCardProps = {
  width?: number;
  height?: number;
  onPress: () => void;
};

const createStyles = (theme: Theme) =>
  StyleSheet.create({
    header: {
      marginBottom: spacing.lg,
    },
    // Dashed and unfilled so it reads as a slot to fill rather than another account.
    card: {
      backgroundColor: 'transparent',
      borderWidth: 1,
      borderStyle: 'dashed',
      borderColor: theme.colors.separator,
      borderRadius: borderRadius.xl,
      shadowOpacity: 0,
    },
    content: {
      flex: 1,
      alignItems: 'center',
      justifyContent: 'center',
      gap: spacing.sm,
    },
    label: {
      color: theme.colors.secondaryText,
    },
  });

const iconSize = 32;

export const AddAccountCard = ({ width = 300, height, onPress }: AddAccountCardProps) => {
  const { t } = useTranslation();
  const styles = useStyles(createStyles);

  return (
    <Pressable accessibilityLabel={t(tk.accountCard.add)} accessibilityRole="button" onPress={onPress} testID="dashboard-add-account-card">
      <AppCard scrollable="horizontal" cardWidth={width} style={[styles.card, { height }]}>
        <View style={styles.content}>
          <SymbolView
            name={AppIconMap.addCircle.ios}
            size={iconSize}
            tintColor={styles.label.color}
            fallback={
              <Image source={AppIconMap.addCircle.android} style={{ width: iconSize, height: iconSize }} tintColor={styles.label.color} />
            }
          />
          <AppText variant="defaultSemiBold" style={styles.label}>
            {t(tk.accountCard.add)}
          </AppText>
        </View>
      </AppCard>
    </Pressable>
  );
};
