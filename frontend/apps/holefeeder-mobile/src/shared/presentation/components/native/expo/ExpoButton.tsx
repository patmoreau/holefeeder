import { Button, ButtonProps } from '@expo/ui';
import React from 'react';

export type ExpoButtonProps = ButtonProps & {};

export const ExpoButton = (props: ExpoButtonProps) => {
  return <Button {...props} />;
};
