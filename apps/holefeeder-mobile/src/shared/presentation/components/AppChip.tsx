import React from 'react';
import { Pressable, Text, View } from 'react-native';
import { useStyles } from '@/shared/theme/core/use-styles';
import { borderRadius, spacing } from '@/types/theme/design-tokens';

export type AppChipProps = {
  label: string;
  selected?: boolean;
  onPress?: () => void;
};

const useTagItemStyles = () =>
  useStyles((theme) => ({
    chip: {
      borderRadius: borderRadius.xl,
      paddingVertical: 6,
      paddingHorizontal: 10,
      borderWidth: 1,
      marginRight: spacing.sm,
      marginVertical: 6,
    },
    chipDefault: {
      backgroundColor: theme.colors.secondary + 20,
      borderColor: theme.colors.secondary,
    },
    chipSelected: {
      backgroundColor: theme.colors.primary + 20,
      borderColor: theme.colors.primary,
    },
    chipPressed: {
      opacity: 0.85,
    },
    chipContent: {
      flexDirection: 'row',
      alignItems: 'center',
    },
    labelDefault: {
      color: theme.colors.secondary,
    },
    labelSelected: {
      color: theme.colors.primary,
    },
    chipText: {
      ...theme.typography.chip,
    },
  }));

export function AppChip({ label, selected = false, onPress }: AppChipProps) {
  const styles = useTagItemStyles();

  return (
    <Pressable
      accessibilityRole="button"
      accessibilityState={{ selected }}
      onPress={onPress}
      style={({ pressed }) => [styles.chip, selected ? styles.chipSelected : styles.chipDefault, pressed ? styles.chipPressed : null]}
    >
      <View style={styles.chipContent}>
        <Text numberOfLines={1} style={[selected ? styles.labelSelected : styles.labelDefault, styles.chipText]}>
          {label}
        </Text>
      </View>
    </Pressable>
  );
}
