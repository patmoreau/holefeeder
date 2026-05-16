import { Button, Text } from '@expo/ui/swift-ui';
import { buttonStyle, controlSize, fixedSize, font, foregroundStyle, padding, tint } from '@expo/ui/swift-ui/modifiers';
import React from 'react';
import { AppChipProps } from '@/shared/presentation/components/AppChip';
import { AppHost } from '@/shared/presentation/components/AppHost.ios';
import { useTheme } from '@/shared/theme/core/use-theme';

export function AppChip({ label, selected = false, onPress }: AppChipProps) {
  const { theme } = useTheme();

  return (
    <AppHost>
      <Button
        onPress={onPress}
        modifiers={[
          buttonStyle('bordered'),
          controlSize('mini'),
          tint(selected ? theme.colors.primary : theme.colors.secondary),
          fixedSize({ horizontal: true, vertical: true }),
          padding({ all: 2 }),
        ]}
      >
        <Text
          modifiers={[
            font({ size: theme.typography.chip.fontSize }),
            foregroundStyle(selected ? theme.colors.primary : theme.colors.secondary),
          ]}
        >
          {label}
        </Text>
      </Button>
    </AppHost>
  );
}
