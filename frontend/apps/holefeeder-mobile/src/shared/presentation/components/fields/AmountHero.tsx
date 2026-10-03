import { AmountField, AmountFieldProps } from '@/shared/presentation/components/fields/AmountField';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { useTheme } from '@/shared/theme/core/use-theme';
import { fontWeight } from '@/types/theme';

export type AmountHeroProps = AmountFieldProps & {
  caption: string;
};

export const AmountHero = ({ caption, ...amountFieldProps }: AmountHeroProps) => {
  const { theme } = useTheme();

  return (
    <AppColumn spacing={2} alignment={'center'}>
      <AppText
        variant={'footnote'}
        textStyle={{
          color: theme.colors.secondaryText,
          fontWeight: fontWeight.semiBold,
        }}
      >
        {caption}
      </AppText>
      <AmountField {...amountFieldProps} />
    </AppColumn>
  );
};

AmountHero.displayName = 'AmountHero';
