import { Button } from '@expo/ui/swift-ui';
import { disabled as disabledModifier } from '@expo/ui/swift-ui/modifiers';
import React from 'react';
import { ExpoButtonProps } from './ExpoButton';

export const ExpoButton = ({ disabled, modifiers = [], variant: _variant, label, children, ...props }: ExpoButtonProps) => {
  const allModifiers = disabled ? [...modifiers, disabledModifier(true)] : modifiers;
  return (
    <Button {...props} label={!children ? label : undefined} modifiers={allModifiers}>
      {children as React.ReactElement | undefined}
    </Button>
  );
};
