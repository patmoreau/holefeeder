import React from 'react';
import { Switch } from 'react-native';

type Props = {
  value: boolean;
  onChange?: (value: boolean) => void;
  readonly?: boolean;
};

export const AppSwitch = ({ value, onChange, readonly }: Props) => {
  return <Switch value={value} onValueChange={onChange} disabled={readonly} />;
};
