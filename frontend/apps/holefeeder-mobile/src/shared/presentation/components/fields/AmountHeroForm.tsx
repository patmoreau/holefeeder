import { ReactNode } from 'react';
import { AmountHero, AmountHeroProps } from '@/shared/presentation/components/fields/AmountHero';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppFieldGroup } from '@/shared/presentation/components/native/AppFieldGroup';
import { AppHostProps, AppNative } from '@/shared/presentation/components/native/AppNative';

type AmountHeroFormProps = AmountHeroProps & {
  header?: ReactNode;
  children: ReactNode;
  hostStyle?: AppHostProps['style'];
};

export const AmountHeroForm = ({ header, children, hostStyle, ...amountHeroProps }: AmountHeroFormProps) => (
  <AppNative style={[{ flex: 1 }, hostStyle]}>
    <AppColumn spacing={8}>
      {header}
      <AmountHero {...amountHeroProps} />
      <AppFieldGroup>{children}</AppFieldGroup>
    </AppColumn>
  </AppNative>
);

AmountHeroForm.displayName = 'AmountHeroForm';
